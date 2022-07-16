using System.ComponentModel.DataAnnotations;
using autodoc.tasks.domain.Common;

namespace autodoc.tasks.domain.Entities;

public class TaskEntity : BaseEntity
{
	[MaxLength(300), MinLength(5)]
	public string Name { get; set; }

	/// <summary>
	/// Datetime of the last update
	/// </summary>
	public DateTime? Updated { get; set; }

	public int? TaskStatusId { get; set; }

	public virtual TaskStatusEntity? Status { get; set; }

	public ICollection<TaskFileEntity>? Files { get; set; }
}