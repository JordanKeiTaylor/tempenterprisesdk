using System;
using System.Linq;
using System.Threading.Tasks;
using Improbable;
using Improbable.Collections;
using Improbable.Worker;
using Improbable.Worker.Query;
using EnterpriseSDK.Extension;
using static EnterpriseSDK.Extension.Optional;

namespace EnterpriseSDK
{
    public class EntityUtil
    {
        private static readonly Option<uint> DefaultTimeout = None<uint>();
        private readonly RequestDispatcher _requestDispatcher;
        private readonly View _view;

        public EntityUtil(RequestDispatcher requestDispatcher, View view)
        {
            _requestDispatcher = requestDispatcher;
            _view = view;
        }

        public virtual void Dispose() {
        }

        public virtual async Task<Option<EntityId>> NextEntityId()
        {
            var reserveResponse = await _requestDispatcher.SendReserveEntityIdRequestAsync(connection =>
            {
                var requestId = connection.SendReserveEntityIdRequest(DefaultTimeout);
                Logging.Debug("Sent ReserveEntityIdRequest with id: " + requestId);
                return requestId.Id;
            });

            if (!isGoodResponse(reserveResponse))
            {
                Logging.Error("No response value - possible timeout: ");
                return None<EntityId>();
            }

            Logging.Debug("Reserved entity id: " + reserveResponse.Value.EntityId.Value);
            return reserveResponse.Value.EntityId;
        }

        public virtual async Task<System.Collections.Generic.IEnumerable<EntityId>> FetchEntityIds(IConstraint constraint)
        {
            var entityIdsOnly = new List<uint>();
            var result = await FetchEntities(constraint, entityIdsOnly);
            return result.Keys;
        }

        public virtual async Task<Map<EntityId, Entity>> FetchEntities(IConstraint constraint, List<uint> componentIds)
        {
            var queryResponse = await _requestDispatcher.SendEntityQueryRequestAsync(connection =>
            {
                var query = new EntityQuery
                {
                    Constraint = constraint,
                    ResultType = new SnapshotResultType(componentIds)
                };

                var requestId = connection.SendEntityQueryRequest(query, DefaultTimeout);
                return requestId.Id;
            });

            if (!isGoodResponse(queryResponse))
            {
                if (queryResponse.HasValue)
                {
                    Logging.Error(
                        $"FetchEntities failed: {queryResponse.Value.StatusCode}, {queryResponse.Value.Message} ");
                }
                else
                {
                    Logging.Error(
                        $"FetchEntities failed: empty query response");
                }
                return new Map<EntityId, Entity>();
            }
            return queryResponse.Value.Result;
        }

        /**
         * Given a task which sends a query, will return an awaitable for the completed task
         */
        protected virtual Task<T> SendEntityQuery<T>(Task<T> queryTask) {
            return queryTask;
        }

        /**
        * Returns the entity specified by entityId with ALL of the components specified in componentIds. If
        * any of the components are missing or the entity query request fails then an empty Option will
        * be returned.
        */
        public virtual async Task<Option<Entity>> FetchEntity(EntityId entityId, List<uint> componentIds, System.Collections.Generic.HashSet<uint> requireAuthorityComponentIds = null)
        {
            Logging.Debug("Fetching entityId " + entityId);
            Entity entity;
            var leftoverComponentIds = new List<uint>(componentIds);
            lock (_view) {
                if (_view.Entities.TryGetValue(entityId, out entity)) {
                    foreach (var component in entity.GetComponents()) {
                        leftoverComponentIds.Remove(component);
                    }
                    if (leftoverComponentIds.Count == 0) {
                        return entity;
                    }
                }
                else {
                    entity = new Entity();
                }
            }

            if (requireAuthorityComponentIds != null && leftoverComponentIds.Any(requireAuthorityComponentIds.Contains)) {
                Logging.Debug("Failed to fetch all components requiring authority from local view");
                return None<Entity>();
            }

            //TODO the components cached here will not get updated as they are changed elsewhere, nor will future queries be made
            Logging.Debug($"Querying leftover components for {entityId}: {string.Join(", ", leftoverComponentIds)}");
            var queryResponse = await SendEntityQuery(_requestDispatcher.SendEntityQueryRequestAsync(connection =>
                {
                    var query = new EntityQuery
                    {
                        Constraint = new EntityIdConstraint(entityId),
                        ResultType = new SnapshotResultType(leftoverComponentIds)
                    };

                    var requestId = connection.SendEntityQueryRequest(query, DefaultTimeout);
                    Logging.Debug("Sent EntityQueryRequest with id: " + requestId);
                    return requestId.Id;
                }));

            if (!isGoodResponse(queryResponse)) {
                Logging.Warn("No response value - possible timeout: ");
                return None<Entity>();
            }

            Logging.Debug("Entity query response count: " + queryResponse.Value.ResultCount);
            var resultEntity = queryResponse.Value.Result[entityId];
            foreach (var componentId in leftoverComponentIds) {
                var resultComponentOption = resultEntity.Get(componentId);
                var resultComponentData = GetComponentDataFromOption(resultComponentOption);
                if (resultComponentData != null) {
                    entity.Add(componentId, resultComponentData);
                }
            }

            if (!EntityContainsAllComponents(entity, componentIds))
            {
                return None<Entity>();
            }
            return entity;
        }

        private object GetComponentDataFromOption(object componentOption) {
            var optionHasValueProperty = componentOption.GetType().GetProperty("HasValue");
            var optionHasValue = (bool)optionHasValueProperty.GetValue(componentOption);
            if (optionHasValue) {
                var optionValueProperty = componentOption.GetType().GetProperty("Value");
                var optionValue = optionValueProperty.GetValue(componentOption);
                return optionValue;
            }
            else {
                return null;
            }
        }

        public virtual bool EntityContainsAllComponents(Entity entity, List<uint> componentIds)
        {
            // TODO expose Contains(uint componentId) method on Entity
            var resultComponentIds = entity.GetComponents().ToDictionary(id => id, id => id);

            return componentIds.All(
                componentId => resultComponentIds.ContainsKey(componentId)
            );
        }

        public virtual async Task<bool> EntityExists(EntityId entityId)
        {
            Logging.Debug("Checking entityId " + entityId + " exists");
            var queryResponse = await _requestDispatcher.SendEntityQueryRequestAsync(connection =>
            {
                var query = new EntityQuery
                {
                    Constraint = new EntityIdConstraint(entityId),
                    ResultType = new CountResultType()
                };

                var requestId = connection.SendEntityQueryRequest(query, DefaultTimeout);
                Logging.Debug("Sent EntityQueryRequest with id: " + requestId);
                return requestId.Id;
            });

            if (!isGoodResponse(queryResponse))
            {
                Logging.Debug("No response value - possible timeout: ");
                return false;
            }

            Logging.Debug("Entity query response count: " + queryResponse.Value.ResultCount);
            return queryResponse.Value.ResultCount > 0;
        }

        public virtual async Task<bool> SendCommand<TC>(EntityId entityId, ICommandRequest<TC> commandRequest)
            where TC : ICommandMetaclass, new()
        {
            var response = await _requestDispatcher.SendCommandRequestAsync<TC>(
                connection =>
                {
                    var requestId = connection.SendCommandRequest(entityId, commandRequest, DefaultTimeout);
                    return requestId.Id;
                });

            if (!isGoodResponse(response)) {
                if (response.HasValue) {
                    Logging.Error($"Command {commandRequest.ToGenericObject().CommandObject} " +
                                 $"(target: {entityId}) {response.Value.RequestId.Id} " +
                                 $"{response.Value.StatusCode}: {response.Value.Message}");
                } else {
                    Logging.Error("Missing command response!");
                }
                return false;
            }
            return true;
        }

        public virtual bool SendComponentUpdate<C>(EntityId entityId, IComponentUpdate<C> update)
            where C : IComponentMetaclass
        {
            return _requestDispatcher.SendComponentUpdate(entityId, update);
        }

        private bool isGoodResponse(Option<EntityQueryResponseOp> response)
        {
            return (response.HasValue && (response.Value.StatusCode == StatusCode.Success));
        }

        private bool isGoodResponse(Option<ReserveEntityIdResponseOp> response)
        {
            return (response.HasValue && (response.Value.StatusCode == StatusCode.Success));
        }

        private bool isGoodResponse<TC>(Option<CommandResponseOp<TC>> response) where TC : ICommandMetaclass, new()
        {
            return (response.HasValue && (response.Value.StatusCode == StatusCode.Success));
        }
    }

}