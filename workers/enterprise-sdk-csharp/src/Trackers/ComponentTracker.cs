using System.Collections.Concurrent;
using System.Collections.Generic;
using Improbable;
using Improbable.Worker;

namespace Improbable.Enterprise.Trackers
{
    internal class ComponentTrackerInternal<T> where T : IComponentMetaclass
    {
        private readonly ConcurrentDictionary<EntityId, IComponentData<T>> _authComponents;
        private readonly bool _onlyAuthoritative;

        private readonly Dispatcher _dispatcher;
        private readonly ulong[] _callbackKeys = new ulong[4];

        public ComponentTrackerInternal(ConcurrentDictionary<EntityId, IComponentData<T>> authComponents, Dispatcher dispatcher, bool onlyAuthoritative)
        {
            _authComponents = authComponents;
            _dispatcher = dispatcher;
            _callbackKeys[0] = dispatcher.OnAuthorityChange<T>(AuthChanged);
            _callbackKeys[1] = dispatcher.OnAddComponent<T>(ComponentAdded);
            _callbackKeys[2] = dispatcher.OnRemoveComponent<T>(ComponentRemoved);
            _callbackKeys[3] = dispatcher.OnComponentUpdate<T>(ComponentUpdated);
            _onlyAuthoritative = onlyAuthoritative;
        }

        ~ComponentTrackerInternal() {
            foreach (var callbackKey in _callbackKeys) {
                _dispatcher.Remove(callbackKey);
            }
        }

        private void AuthChanged(AuthorityChangeOp op)
        {
            if (_onlyAuthoritative && !op.HasAuthority)
            {
                IComponentData<T> removedValue;
                _authComponents.TryRemove(op.EntityId, out removedValue);
            }
            else
            {
                _authComponents.AddOrUpdate(op.EntityId, addId => default(IComponentData<T>),
                                            (existingId, existingData) => existingData);
            }
        }

        private void ComponentAdded(AddComponentOp<T> op)
        {
            _authComponents.AddOrUpdate(op.EntityId, addId => (IComponentData<T>)op.Data, (updateId, existingData) => (IComponentData<T>)op.Data);
        }

        private void ComponentRemoved(RemoveComponentOp op)
        {
            IComponentData<T> removedValue;
            _authComponents.TryRemove(op.EntityId, out removedValue);
        }

        private void ComponentUpdated(ComponentUpdateOp<T> op)
        {
            IComponentData<T> existingValue;
            if (_authComponents.TryGetValue(op.EntityId, out existingValue))
            {
                op.Update.ApplyTo((IComponentData<T>)existingValue);
            }
        }
    }

    public class ComponentTracker<T> where T : IComponentMetaclass
    {
        private readonly ConcurrentDictionary<EntityId, IComponentData<T>> _authComponents;

        private ComponentTrackerInternal<T> _tracker;

        public ComponentTracker(Dispatcher dispatcher, bool onlyAuthoritative = true)
        {
            _authComponents = new ConcurrentDictionary<EntityId, IComponentData<T>>();
            _tracker = new ComponentTrackerInternal<T>(_authComponents, dispatcher, onlyAuthoritative);
        }

        public IDictionary<EntityId, IComponentData<T>> Entities()
        {
            return new Dictionary<EntityId, IComponentData<T>>(_authComponents);
        }
    }
}
