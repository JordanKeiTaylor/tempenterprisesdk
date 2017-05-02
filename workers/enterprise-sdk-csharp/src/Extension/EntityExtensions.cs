using System;
using Improbable.Collections;
using Improbable.Worker;
using Improbable.Worker.Internal;

namespace WorkerAPIFacade.Extension
{
    public static class EntityExtensions {

        public static object Get(this Entity entity, uint componentId) {
            var componentMetaclass = ComponentDatabase.IdToMetaclass(componentId).GetType();
            var getComponentMethod =
                entity.GetType().GetMethod("Get").MakeGenericMethod(new Type[] {componentMetaclass});
            var resultComponent = getComponentMethod.Invoke(entity, null);
            return resultComponent;
        }

        public static void Add(this Entity entity, uint componentId, object value) {
            var componentMetaclass = ComponentDatabase.IdToMetaclass(componentId).GetType();
            var addComponentMethod =
                entity.GetType().GetMethod("Add").MakeGenericMethod(new Type[] { componentMetaclass });
            addComponentMethod.Invoke(entity, new object[] { value });
        }

        public static bool TryGetComponent<CT>(this Option<Entity> entity, out IComponentData<CT> data) where CT : IComponentMetaclass
        {
            if (entity.HasValue) {
                return entity.Value.TryGetComponent<CT>(out data);
            }
            data = null;
            return false;
        }

        public static bool TryGetComponent<CT>(this Entity entity, out IComponentData<CT> data) where CT : IComponentMetaclass
        {
            if (entity.Get<CT>().HasValue)
            {
                data = entity.Get<CT>().Value;
                return true;
            }
            data = null;
            return false;
        }
    }
}