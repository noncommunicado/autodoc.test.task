using autodoc.tasks.domain.Dto.TaskFile;
using autodoc.tasks.domain.Dto.TaskStatus;

namespace autodoc.tasks.domain.Dto.Task;

public sealed record TaskDto(
	int Id,
	string Name,
	DateTime? Created,
	DateTime? Updated,
	int? StatusId,
	TaskStatusExtendedDto? Status = null,
	IEnumerable<TaskFileDto>? Files = null
);