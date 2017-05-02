using Improbable;

namespace Improbable.Enterprise.Extension
{
    public static class EntityIdExtensions
    {
        public static bool IsValid(this EntityId entityId)
        {
            return entityId.Id >= 0;
        }
    }
}