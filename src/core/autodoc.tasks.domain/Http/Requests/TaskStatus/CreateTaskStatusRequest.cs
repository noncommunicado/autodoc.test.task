using System.ComponentModel.DataAnnotations;

namespace autodoc.tasks.domain.Http.Requests.TaskStatus;

public class CreateTaskStatusRequest
{
	[MaxLength(100), MinLength(2)]
	public string Name { get; set; }

	[MaxLength(100), MinLength(2)]
	public string EnAlias { get; set; }

	[MaxLength(1000)]
	public string? Description { get; set; }
}