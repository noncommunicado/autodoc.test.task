using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.Task.Queries;

public sealed record GetTaskByIdQuery(int Id) : IRequest<TaskEntity?>;

public sealed class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskEntity?>
{
	private readonly IMainDbContext _context;
	public GetTaskByIdQueryHandler(IMainDbContext context)
	{
		_context = context;
	}
	
	public async Task<TaskEntity?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
	{
		var entity = await _context.Tasks
			.Include(x => x.Status)
			.Include(x => x.Files)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

		return entity;
	}
}
