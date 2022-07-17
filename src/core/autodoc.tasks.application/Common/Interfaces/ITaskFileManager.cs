using Microsoft.AspNetCore.Http;

namespace autodoc.tasks.application.Common.Interfaces;

public interface ITaskFileManager
{
	bool IsInternalFileExits(string fileName);

	FileStream GetFileStream(string fileName);

	Task SaveFileAsync(IFormFile file, string uniqName, CancellationToken ct = new());

	void DeleteFile(string fileName);

	/// <summary>
	/// No exception will be thrown
	/// </summary>
	bool TryDeleteFile(string fileName);
}