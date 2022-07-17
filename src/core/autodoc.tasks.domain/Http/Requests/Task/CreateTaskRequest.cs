using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace autodoc.tasks.domain.Http.Requests.Task;

public sealed class CreateTaskRequest
{
	[MaxLength(300), MinLength(5)]
	public string Name { get; set; }
	public int? StatusId { get; set; }
	public List<IFormFile>? Files { get; set; } = null;
}