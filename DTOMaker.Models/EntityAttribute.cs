using System;

namespace DTOMaker.Models
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class EntityAttribute : Attribute
    {
        public readonly int EntityId;

        public EntityAttribute(int entityId)
        {
            EntityId = entityId;
        }
    }
}
