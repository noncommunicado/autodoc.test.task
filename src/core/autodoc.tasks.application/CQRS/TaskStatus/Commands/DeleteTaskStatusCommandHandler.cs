using System.Net;
using autodoc.tasks.application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.TaskStatus.Commands;

public sealed record DeleteTaskStatusCommand(int Id) : IRequest;

public sealed class DeleteTaskStatusCommandHandler : IRequestHandler<DeleteTaskStatusCommand>
{
	private readonly IMainDbContext _context;
	public DeleteTaskStatusCommandHandler(IMainDbContext context)
	{
		_context = context;
	}

	public async Task<Unit> Handle(DeleteTaskStatusCommand request, CancellationToken cancellationToken)
	{
		var entity = await _context.TaskStatuses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
		if (entity is null)
			throw new HttpRequestException("Not found", null, HttpStatusCode.NotFound);
		_context.TaskStatuses.Remove(entity);
		await _context.SaveChangesAsync(cancellationToken);
		return Unit.Value;
	}
}