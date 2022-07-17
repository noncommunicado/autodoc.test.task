using System.Net;
using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.application.CQRS.TaskStatus.Commands;
using autodoc.tasks.domain.Http.Requests.Task;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.Task.Commands;

public sealed record UpdateTaskCommand(UpdateTaskRequest Model) : IRequest;
public sealed class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
	private readonly IMainDbContext _context;
	public UpdateTaskCommandHandler(IMainDbContext context)
	{
		_context = context;
	}

	public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
	{
		var entity = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

		if (entity == null)
			throw new HttpRequestException("Task not found", null, HttpStatusCode.NotFound);

		if(request.Model.StatusId.HasValue)
			if(await new IsTaskStatusExistCommandHandler(_context)
				   .Handle(new IsTaskStatusExistCommand(request.Model.StatusId.Value), cancellationToken) is false)
				throw new HttpRequestException("Task status not found", null, HttpStatusCode.BadRequest);

		entity.Name = request.Model.Name;
		entity.StatusId = request.Model.StatusId;

		await _context.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}