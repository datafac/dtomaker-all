using System;

namespace DTOMaker.Models
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class EntityAttribute : Attribute
    {
        public readonly int EntityId;
        public readonly LayoutMethod LayoutMethod;

        public EntityAttribute(int entityId, LayoutMethod layoutMethod = LayoutMethod.Undefined) // todo default to Compact
        {
            EntityId = entityId;
            LayoutMethod = layoutMethod;
        }
    }
}
