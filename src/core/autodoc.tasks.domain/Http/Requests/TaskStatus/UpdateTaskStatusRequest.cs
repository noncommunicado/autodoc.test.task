using System.ComponentModel.DataAnnotations;

namespace autodoc.tasks.domain.Http.Requests.TaskStatus;

public class UpdateTaskStatusRequest : CreateTaskStatusRequest
{
	[Required]
	public int Id { get; set; }
}