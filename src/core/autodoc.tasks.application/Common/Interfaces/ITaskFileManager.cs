using autodoc.tasks.domain.Dto.TaskFile;
using Microsoft.AspNetCore.Http;

namespace autodoc.tasks.application.Common.Interfaces;

public interface ITaskFileManager
{
	bool IsInternalFileExits(string fileName);
	Task<FileStream> GetFileStreamAsync(TaskFileExtendedDto fileDto);
	Task SaveFileAsync(IFormFile file, string uniqName, CancellationToken ct = new());
	void DeleteFile(string fileName);
	bool TryDeleteFile(string fileName);
}