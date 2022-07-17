namespace autodoc.tasks.domain.Dto.TaskFile;

public record TaskFileDto(
	int Id,
	string FileName,
	string? FileExtension,
	long Size
);