namespace autodoc.tasks.application.Common.Interfaces;

public interface IArchivatorService : IDisposable
{
	Task AddFileAsync(string fileName, string folder = "");
	Task AddFileAsync(string fileName, Stream stream);
	byte[] GetArchiveBytesAndCloseArchive();
	Stream GetArchiveStream(string fileFullName);
	Task SaveArchiveAsync(string directory, string fileName);
	Task SaveArchiveAsync(string fileFullName);
}