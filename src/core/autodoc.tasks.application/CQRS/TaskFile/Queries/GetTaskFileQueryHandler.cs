using System.Net;
using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.TaskFile.Queries;

public sealed record GetTaskFileQuery(int Id) : IRequest<(FileStream Stream, string FileName)>;
public sealed class GetTaskFileQueryHandler : IRequestHandler<GetTaskFileQuery, (FileStream Stream, string FileName)>
{
	private readonly IMainDbContext _context;
	private readonly ITaskFileManager _fileManager;
	public GetTaskFileQueryHandler(IMainDbContext context, ITaskFileManager fileManager)
	{
		_context = context;
		_fileManager = fileManager;
	}

	public async Task<(FileStream Stream, string FileName)> Handle(GetTaskFileQuery request, CancellationToken cancellationToken)
	{
		TaskFileEntity? entity = await _context.TaskFiles
			.FirstOrDefaultAsync(x => x.Id == request.Id,
			cancellationToken: cancellationToken);

		if (entity is null)
			throw new HttpRequestException("File not found", null, HttpStatusCode.NotFound);

		return new (
			_fileManager.GetFileStream(entity.LocalStorageFileId.ToString("N"))
			, GetFileName(entity));
	}

	private string GetFileName(TaskFileEntity entity)
	{
		string result = string.Empty;
		if (string.IsNullOrEmpty(entity.FileName) is false) result += entity.FileName;
		if (string.IsNullOrEmpty(entity.FileExtension) is false) result += $".{entity.FileExtension}";
		return result;
	}
}