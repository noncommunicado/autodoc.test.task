using System.Net;
using autodoc.tasks.application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.Task.Commands;

public sealed record DeleteTaskCommand(int Id, bool IsDeleteFilesToo = false) : IRequest;

public sealed class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
	private readonly IMainDbContext _context;
	private readonly ITaskFileManager _fileManager;
	public DeleteTaskCommandHandler(IMainDbContext context, ITaskFileManager fileManager)
	{
		_context = context;
		_fileManager = fileManager;
	}

	public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
	{
		var entity = await _context.Tasks
			.Include(x => x.Files)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
		if (entity == null)
			throw new HttpRequestException("Task not found", null, HttpStatusCode.NotFound);

		_context.Tasks.Remove(entity);
		await _context.SaveChangesAsync(cancellationToken);

		if (request.IsDeleteFilesToo && entity.Files != null)
#pragma warning disable CS4014
			new TaskFactory().StartNew(() =>
			{
				foreach (var file in entity.Files)
					_fileManager.DeleteFile(file.LocalStorageFileId.ToString("N"));
			}, cancellationToken).ConfigureAwait(false);
#pragma warning restore CS4014

		return Unit.Value;
	}
}