using System.IO.Compression;
using System.Text;
using autodoc.tasks.application.Common.Interfaces;

namespace autodoc.tasks.persistence.Archivator;

public sealed class ArchivatorService : IArchivatorService
{
	private MemoryStream _memoryStream;
	private ZipArchive _archive;

	private bool IsDisposed() => _memoryStream is null && _archive is null;

	public ArchivatorService Init()
	{
		_memoryStream = new MemoryStream();
		_archive = new ZipArchive(_memoryStream, ZipArchiveMode.Create, false, Encoding.UTF8);
		return this;
	}

	public async Task AddFileAsync(string fileName, string folder = "")
	{
		if (IsDisposed()) Init();

		var fInfo = new FileInfo(fileName);

		if (fInfo.Exists == false) throw new FileNotFoundException();

		var entryName = string.IsNullOrEmpty(folder) ? fInfo.Name : $"{folder.TrimStart('\\')}\\{fInfo.Name}";
		_archive.CreateEntryFromFile(fInfo.FullName, entryName);
	}

	public async Task AddFileAsync(string fileName, Stream stream)
	{
		if (IsDisposed()) Init();

		var entry = _archive.CreateEntry(fileName, CompressionLevel.Fastest);
		await using var entryStream = entry.Open();
		await stream.CopyToAsync(entryStream);
	}

	public byte[] GetArchiveBytesAndCloseArchive()
	{
		_archive.Dispose();
		return _memoryStream.ToArray();
	}

	public Stream GetArchiveStream(string fileFullName)
	{
		if (IsDisposed()) Init();

		var fileStream = new FileStream(fileFullName, FileMode.Create);
		_memoryStream.Seek(0, SeekOrigin.Begin);
		_memoryStream.CopyTo(fileStream);
		return fileStream;
	}

	public async Task SaveArchiveAsync(string directory, string fileName)
	{
		if (IsDisposed()) Init();

		if (Directory.Exists(directory) == false) throw new DirectoryNotFoundException();

		string fileFullName = directory.TrimEnd('/').TrimEnd('\\') + fileName.TrimStart('/').TrimStart('\\');
		await SaveArchiveAsync(fileFullName);
	}

	public async Task SaveArchiveAsync(string fileFullName)
	{
		if (IsDisposed()) Init();

		using (var fileStream = new FileStream(fileFullName, FileMode.Create))
		{
			_memoryStream.Seek(0, SeekOrigin.Begin);
			await _memoryStream.CopyToAsync(fileStream);
		}
	}

	public void Dispose()
	{
		if (IsDisposed()) return;

		try
		{
			_archive.Dispose();
			_memoryStream.Dispose();
		}
		catch
		{
			//swallow
		}
	}
	~ArchivatorService() => Dispose();
}