using System.Collections.Concurrent;
using System.Net;
using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.TaskFile.Commands;

public sealed record AddFilesToTaskCommand(int TaskId, IEnumerable<IFormFile> Files) : IRequest;
public sealed class AddFilesToTaskCommandHandler : IRequestHandler<AddFilesToTaskCommand>
{
	private readonly IMainDbContext _context;
	private readonly ITaskFileManager _fileManager;
	public AddFilesToTaskCommandHandler(IMainDbContext context, ITaskFileManager fileManager)
	{
		_context = context;
		_fileManager = fileManager;
	}

	public async Task<Unit> Handle(AddFilesToTaskCommand request, CancellationToken cancellationToken)
	{
		var entity = await _context.Tasks
			.FirstOrDefaultAsync(x => x.Id == request.TaskId,
			cancellationToken: cancellationToken);

		if (entity == null)
			throw new HttpRequestException("Task not found", null, HttpStatusCode.NotFound);

		BlockingCollection<TaskFileEntity> savedFiles = new();
		if (request.Files.Any())
		{
			await Parallel.ForEachAsync(request.Files, cancellationToken, async (file, token) =>
			{
				if (file.Length <= 0) return;
				Guid localStorageFileId = Guid.NewGuid();
				await _fileManager.SaveFileAsync(file, localStorageFileId.ToString("N"), token);
				savedFiles.Add(new TaskFileEntity
				{
					TaskId = entity.Id,
					Created = DateTime.Now,
					FileExtension = Path.GetExtension(file.FileName).Replace(".", string.Empty),
					FileName = Path.GetFileNameWithoutExtension(file.FileName),
					LocalStorageFileId = localStorageFileId,
					Size = file.Length
				}, token);
			});
		}

		await _context.TaskFiles.AddRangeAsync(savedFiles, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}