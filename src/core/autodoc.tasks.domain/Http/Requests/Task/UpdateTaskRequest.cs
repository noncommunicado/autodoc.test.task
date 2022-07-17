using System.ComponentModel.DataAnnotations;

namespace autodoc.tasks.domain.Http.Requests.Task;

public sealed class UpdateTaskRequest
{
	[Required]
	public int Id { get; set; }

	[Required, MaxLength(300), MinLength(5)]
	public string Name { get; set; }

	public int? StatusId { get; set; }
}