using System;

namespace DTOMaker.Models
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class IdAttribute : Attribute
    {
        public readonly int EntityId;

        public IdAttribute(int entityId)
        {
            EntityId = entityId;
        }
    }
}
