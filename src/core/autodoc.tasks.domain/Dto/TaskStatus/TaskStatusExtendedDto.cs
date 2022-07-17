namespace autodoc.tasks.domain.Dto.TaskStatus;

public sealed record TaskStatusExtendedDto(int Id, string Name, string EnAlias, string? Description = null);