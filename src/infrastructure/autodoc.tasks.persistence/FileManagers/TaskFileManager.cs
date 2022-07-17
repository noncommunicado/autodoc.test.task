using autodoc.tasks.application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace autodoc.tasks.persistence.FileManagers;

public sealed class TaskFileManager : ITaskFileManager
{
	private DirectoryInfo StoreDirectory => new (Path.Combine(AppContext.BaseDirectory, "TasksFiles"));

	public bool IsInternalFileExits(string fileName)
		=> StoreDirectory.GetFiles(fileName).FirstOrDefault() is null;

	/// <exception cref="FileNotFoundException"></exception>
	public FileStream GetFileStream(string fileName)
	{
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

	public async Task SaveFileAsync(byte[] bytes, string uniqName, CancellationToken ct = new())
	{
		if (StoreDirectory.Exists is false) StoreDirectory.Create();

		string filePath = Path.Combine(StoreDirectory.FullName, uniqName);
		await using FileStream stream = new FileStream(filePath, FileMode.Create);
		await stream.WriteAsync(bytes, 0, bytes.Length, ct);
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