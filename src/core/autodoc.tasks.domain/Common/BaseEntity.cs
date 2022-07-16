using System.ComponentModel.DataAnnotations;

namespace autodoc.tasks.domain.Common;

public abstract class BaseEntity
{
	[Key]
	public int Id { get; set; }

	public DateTime? Created { get; set; } = DateTime.Now;
}