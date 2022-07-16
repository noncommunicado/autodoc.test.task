using System.ComponentModel.DataAnnotations;
using autodoc.tasks.domain.Common;

namespace autodoc.tasks.domain.Entities;

public class TaskStatusEntity : BaseEntity
{
	[MaxLength(100), MinLength(2)]
	public string Name { get; set; }

	/// <summary>
	/// En alias of task name 
	/// </summary>
	[MaxLength(100), MinLength(2)]
	public string EnAlias { get; set; }

	[MaxLength(1000)]
	public string? Description { get; set; }
}