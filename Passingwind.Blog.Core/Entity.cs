using System;

namespace Passingwind.Blog
{
    public abstract class AuditedEntity : CreationEntity, IHasModificationTime
    {
        public DateTime? LastModificationTime { get; set; }
    }

    public abstract class CreationEntity : Entity, IHasCreationTime
    {
        public DateTime CreationTime { get; set; }

        public CreationEntity()
        {
            this.CreationTime = DateTime.Now;
        }
    }

    /// <summary>
    ///  Entity basic class
    /// </summary>
    public abstract class Entity : Entity<string>
    {
        public Entity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public abstract class Entity<TKey> : IEntity<TKey>
    {
        public virtual TKey Id { get; set; }
    }

    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }

    public interface IHasModificationTime
    {
        /// <summary>
        /// The last modified time for this entity.
        /// </summary>
        DateTime? LastModificationTime { get; set; }
    }

    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }

    public interface IHasDeletionTime : ISoftDelete
    {
        /// <summary>
        /// Deletion time of this entity.
        /// </summary>
        DateTime? DeletionTime { get; set; }
    }

    public interface IHasCreationTime
    {
        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        DateTime CreationTime { get; set; }
    }
}