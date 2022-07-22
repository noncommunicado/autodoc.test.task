using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.TaskStatus.Queries;

public sealed record GetTaskStatusByIdQuery(int Id) : IRequest<TaskStatusEntity?>;

public sealed class GetTaskStatusByIdQueryHandler : IRequestHandler<GetTaskStatusByIdQuery, TaskStatusEntity?>
{
	private readonly IMainDbContext _context;
	public GetTaskStatusByIdQueryHandler(IMainDbContext context)
	{
		_context = context;
	}

	public async Task<TaskStatusEntity?> Handle(GetTaskStatusByIdQuery request, CancellationToken cancellationToken)
		=> await _context.TaskStatuses
			.AsNoTrackingWithIdentityResolution()
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
}