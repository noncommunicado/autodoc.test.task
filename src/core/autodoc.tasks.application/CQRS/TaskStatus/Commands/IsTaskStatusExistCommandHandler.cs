using autodoc.tasks.application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.TaskStatus.Commands;

public sealed record IsTaskStatusExistCommand(int Id) : IRequest<bool>;
public sealed class IsTaskStatusExistCommandHandler : IRequestHandler<IsTaskStatusExistCommand, bool>
{
	private readonly IMainDbContext _context;
	public IsTaskStatusExistCommandHandler(IMainDbContext context)
	{
		_context = context;
	}

	public async Task<bool> Handle(IsTaskStatusExistCommand request, CancellationToken cancellationToken)
	{
		return await _context.TaskStatuses.AnyAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
	}
}