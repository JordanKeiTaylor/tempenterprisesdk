using System;
using System.Threading.Tasks;
using Improbable;
using Improbable.Collections;
using Improbable.Worker;

namespace EnterpriseSDK
{
    public class RequestDispatcher
    {
        private readonly Dispatcher _dispatcher;
        private readonly Connection _connection;

        public RequestDispatcher(Connection connection, Dispatcher dispatcher)
        {
            _connection = connection;
            _dispatcher = dispatcher;
        }

        public virtual bool SendComponentUpdate<C>(EntityId entityId, IComponentUpdate<C> update)
            where C : IComponentMetaclass
        {
            //TODO Have some mechanism for reliable component updates - https://support.improbable.io/t/are-component-updates-guaranteed/1060
            lock (_connection)
            {
                _connection.SendComponentUpdate(entityId, update);
            }
            //TODO wait for next ComponentUpdateOp<C>:
            // If next update contains update, all is good
            // Else if next update looks like oldData, it failed
            return true;
        }

        public virtual Task<Option<CommandResponseOp<C>>> SendCommandRequestAsync<C>(
            Func<Connection, uint> sender) where C : ICommandMetaclass, new()
        {
            return SendWorldCommandAsync<CommandResponseOp<C>>(sender, op => op.RequestId.Id, op => op.StatusCode,
                    _dispatcher.OnCommandResponse);
        }

        public virtual void SendCommandResponse<C>(RequestId<IncomingCommandRequest<C>> requestId,
            ICommandResponse<C> response) where C : ICommandMetaclass, new()
        {
            lock (_connection)
            {
                _connection.SendCommandResponse(requestId, response);
            }
        }

        public virtual Task<Option<CreateEntityResponseOp>> SendCreateEntityRequestAsync(Func<Connection, uint> sender)
        {
            return SendWorldCommandAsync<CreateEntityResponseOp>(sender, op => op.RequestId.Id, op => op.StatusCode,
                _dispatcher.OnCreateEntityResponse);
        }

        public virtual Task<Option<DeleteEntityResponseOp>> SendDeleteEntityRequestAsync(Func<Connection, uint> sender)
        {
            return SendWorldCommandAsync<DeleteEntityResponseOp>(sender, op => op.RequestId.Id, op => op.StatusCode,
                _dispatcher.OnDeleteEntityResponse);
        }

        public virtual Task<Option<ReserveEntityIdResponseOp>> SendReserveEntityIdRequestAsync(
            Func<Connection, uint> sender)
        {
            return SendWorldCommandAsync<ReserveEntityIdResponseOp>(sender, op => op.RequestId.Id, op => op.StatusCode,
                _dispatcher.OnReserveEntityIdResponse);
        }

        public virtual Task<Option<EntityQueryResponseOp>> SendEntityQueryRequestAsync(Func<Connection, uint> sender)
        {
            return SendWorldCommandAsync<EntityQueryResponseOp>(sender, op => op.RequestId.Id, op => op.StatusCode,
                _dispatcher.OnEntityQueryResponse);
        }

        private async Task<Option<TResponse>> SendWorldCommandAsync<TResponse>(
            Func<Connection, uint> sender,
            Func<TResponse, uint> opToRequestId,
            Func<TResponse, StatusCode> opToStatusCode,
            Func<Action<TResponse>, ulong> registerCallback,
            int maxAttempts = 5
        )
        {
            Option<TResponse> response = null;
            var attempts = 0;

            while (RequestFailed(response, opToStatusCode) && ShouldAttemptRequest(ref attempts, maxAttempts))
            {
                var waitForResponse = new TaskCompletionSource<TResponse>();
                UInt32 requestId;
                lock (_connection)
                {
                    requestId = sender(_connection);
                }
                UInt64 responseCallbackKey;
                UInt64 disconnectCallbackKey;
                lock (_dispatcher)
                {
                    responseCallbackKey = registerCallback(op =>
                    {
                        if (opToRequestId(op) == requestId)
                        {
                            waitForResponse.SetResult(op);
                        }
                    });
                    disconnectCallbackKey = _dispatcher.OnDisconnect(op => waitForResponse.SetCanceled());
                }

                try {
                    response = await waitForResponse.Task;
                }
                catch (OperationCanceledException) {
                    response = null;
                }

                try
                {
                    lock (_dispatcher)
                    {
                        _dispatcher.Remove(responseCallbackKey);
                        _dispatcher.Remove(disconnectCallbackKey);
                    }
                }
                catch (ArgumentException e)
                {
                    Logging.Error($"Removing of response callbacks {responseCallbackKey} or {disconnectCallbackKey} failed: {e}");
                }
            }
            return response;
        }

        private static bool RequestFailed<TResponse>(Option<TResponse> response,
            Func<TResponse, StatusCode> opToStatusCode)
        {
            return !response.HasValue || opToStatusCode(response.Value) != StatusCode.Success;
        }

        private static bool ShouldAttemptRequest(ref int attempts, int maxAttempts)
        {
            return attempts++ < maxAttempts;
        }
    }
}