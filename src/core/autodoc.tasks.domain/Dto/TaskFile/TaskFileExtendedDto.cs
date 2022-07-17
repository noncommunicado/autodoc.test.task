namespace autodoc.tasks.domain.Dto.TaskFile;

public sealed record TaskFileExtendedDto(
	int Id,
	string FileName,
	string? FileExtension,
	long Size,
	Guid LocalStorageFileId,
	bool IsStoredCompressed
) : TaskFileDto(
	Id,
	FileName,
	FileExtension,
	Size
);