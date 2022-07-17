using System.Net;
using autodoc.tasks.application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.TaskFile.Commands;

public sealed record DeleteTaskFileCommand(int Id) : IRequest;
public sealed class DeleteTaskFileCommandHandler : IRequestHandler<DeleteTaskFileCommand>
{
	private readonly IMainDbContext _context;
	private readonly ITaskFileManager _fileManager;
	public DeleteTaskFileCommandHandler(IMainDbContext context, ITaskFileManager fileManager)
	{
		_context = context;
		_fileManager = fileManager;
	}

	public async Task<Unit> Handle(DeleteTaskFileCommand request, CancellationToken cancellationToken)
	{
		var entity = await _context.TaskFiles.FirstOrDefaultAsync(x => x.Id == request.Id,
			cancellationToken: cancellationToken);

		if (entity == null)
			throw new HttpRequestException("Task file not found", null, HttpStatusCode.NotFound);

		_context.TaskFiles.Remove(entity);
		await _context.SaveChangesAsync(cancellationToken);

		_fileManager.TryDeleteFile(entity.LocalStorageFileId.ToString("N"));

		return Unit.Value;
	}
}