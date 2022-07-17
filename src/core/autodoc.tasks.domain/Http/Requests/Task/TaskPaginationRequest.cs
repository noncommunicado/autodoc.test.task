using System.ComponentModel.DataAnnotations;
using autodoc.tasks.domain.Common;

namespace autodoc.tasks.domain.Http.Requests.Task;

public sealed class TaskPaginationRequest : BasePaginationRequest
{
	[MaxLength(100)]
	public string? Search { get; set; } = null;
	public int? Status { get; set; } = null;
	public int? WithFilesCount { get; set; } = null;
	public DateTime? From { get; set; } = null;
	public DateTime? To { get; set; } = null;
}