using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.TaskStatus.Queries;

public sealed record GetAllTaskStatusesQuery(string? SearchFilter = null) : IRequest<IEnumerable<TaskStatusEntity>>;

public sealed class GetAllTaskStatusesQueryHandler : IRequestHandler<GetAllTaskStatusesQuery, IEnumerable<TaskStatusEntity>>
{
	private readonly IMainDbContext _context;
	public GetAllTaskStatusesQueryHandler(IMainDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<TaskStatusEntity>> Handle(GetAllTaskStatusesQuery request,
		CancellationToken cancellationToken)
		=> await _context.TaskStatuses
			.Where(x => request.SearchFilter == null || (
					x.Name.ToLower().Contains(request.SearchFilter.ToLower()) || x.EnAlias.ToLower().Contains(request.SearchFilter.ToLower())
				))
			.ToArrayAsync(cancellationToken: cancellationToken);
}