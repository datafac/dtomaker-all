using System;

namespace DTOMaker.Models
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class EntityAttribute : Attribute
    {
        /// <summary>
        /// The unique identifier for the entity associated with the interface.
        /// Todo - upgrade to GUIDs.
        /// </summary>
        public readonly int EntityId;

        public EntityAttribute(int entityId)
        {
            EntityId = entityId;
        }
    }
}
