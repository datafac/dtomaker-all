using System;

namespace DTOMaker.Runtime
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Returns an unfrozen copy of the entity if it is frozen; otherwise, returns the original entity.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the entity is frozen and an unfrozen copy cannot be created.</exception>
        public static TEntity Unfrozen<TEntity>(this TEntity entity) where TEntity : class, IEntityBase
        {
            return entity.IsFrozen
                ? entity.PartCopy() as TEntity ?? throw new InvalidOperationException("Failed to create unfrozen copy.")
                : entity;
        }
    }
}
