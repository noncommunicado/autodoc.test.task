using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.domain.Dto.TaskFile;
using Microsoft.AspNetCore.Http;

namespace autodoc.tasks.persistence.FileManagers;

public sealed class TaskFileManager : ITaskFileManager
{
	private DirectoryInfo StoreDirectory => new (Path.Combine(AppContext.BaseDirectory, "TasksFiles"));

	public bool IsInternalFileExits(string fileName)
		=> StoreDirectory.GetFiles(fileName).FirstOrDefault() is null;

	/// <exception cref="FileNotFoundException"></exception>
	public async Task<FileStream> GetFileStreamAsync(TaskFileExtendedDto fileDto)
	{
		string fileName = fileDto.LocalStorageFileId.ToString("N");
		FileInfo? fileInfo = StoreDirectory.GetFiles(fileName).FirstOrDefault();
		if (fileInfo is null) throw new FileNotFoundException();

		return fileInfo.OpenRead();
	}

	public async Task SaveFileAsync(IFormFile file, string uniqName, CancellationToken ct = new())
	{
		if (StoreDirectory.Exists is false) StoreDirectory.Create();

		string filePath = Path.Combine(StoreDirectory.FullName, uniqName);
		await using Stream fileStream = new FileStream(filePath, FileMode.Create);
		await file.CopyToAsync(fileStream, ct);
	}

	public void DeleteFile(string fileName)
	{
		var fileToDelete = StoreDirectory.GetFiles(fileName).FirstOrDefault();
		if(fileToDelete is null) throw new FileNotFoundException();
		fileToDelete.Delete();
	}

	public bool TryDeleteFile(string fileName)
	{
		var fileToDelete = StoreDirectory.GetFiles(fileName).FirstOrDefault();
		try
		{
			fileToDelete?.Delete();
		}
		catch
		{
			// swallow
			return false;
		}

		return true;
	}
}