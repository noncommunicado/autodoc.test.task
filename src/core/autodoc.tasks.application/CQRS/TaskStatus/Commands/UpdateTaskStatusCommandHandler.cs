using System.Net;
using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.domain.Http.Requests.TaskStatus;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.TaskStatus.Commands;

public sealed record UpdateTaskStatusCommand(UpdateTaskStatusRequest Model) : IRequest;

public sealed class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand>
{
	private readonly IMainDbContext _context;
	public UpdateTaskStatusCommandHandler(IMainDbContext context)
	{
		_context = context;
	}

	public async Task<Unit> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
	{
		var entity = await _context.TaskStatuses.FirstOrDefaultAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);
		if (entity is null)
			throw new HttpRequestException("Not found", null, HttpStatusCode.NotFound);
		entity.EnAlias = request.Model.EnAlias;
		entity.Name = request.Model.Name;
		entity.Description = request.Model.Description;

		try
		{
			await _context.SaveChangesAsync(cancellationToken);
		}
		catch (DbUpdateException ex)
		{
			throw new HttpRequestException("Status with same name already exist", ex, HttpStatusCode.Conflict);
		}

		return Unit.Value;
	}
}