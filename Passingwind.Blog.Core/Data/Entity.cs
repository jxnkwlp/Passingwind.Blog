using System;

namespace Passingwind.Blog.Data
{
	public interface IEntity<TKey>
	{
		TKey Id { get; set; }
	}

	public interface IHasEntityCreationTime
	{
		DateTime CreationTime { get; set; }
	}

	public interface IHasLastModificationTime
	{
		DateTime? LastModificationTime { get; set; }
	}

	/// <summary>
	///  base entity with key is <see cref="TKey"/>
	/// </summary>
	public abstract class Entity<TKey> : IEntity<TKey>
	{
		public TKey Id { get; set; }
	}

	/// <summary>
	///  base entity with key is <see cref="int"/>
	/// </summary>
	public abstract class Entity : Entity<int>
	{

	}

	public abstract class AuditTimeEntity<TKey> : Entity<TKey>, IHasEntityCreationTime, IHasLastModificationTime
	{
		public DateTime CreationTime { get; set; } = DateTime.Now;
		public DateTime? LastModificationTime { get; set; }
	}

	public abstract class AuditTimeEntity : AuditTimeEntity<int>
	{

	}
}
