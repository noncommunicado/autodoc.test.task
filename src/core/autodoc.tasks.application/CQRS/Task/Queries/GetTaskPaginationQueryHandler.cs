using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.domain.Dto.Task;
using autodoc.tasks.domain.Http.Requests.Task;
using autodoc.tasks.domain.Http.Responses.Pagination;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.Task.Queries;

public sealed record GetTaskPaginationQuery(TaskPaginationRequest Model) : IRequest<PaginationResponse<TaskDto>>;

public sealed class GetTaskPaginationQueryHandler : IRequestHandler<GetTaskPaginationQuery, PaginationResponse<TaskDto>>
{
	private readonly IMainDbContext _context;
	private readonly IMapper _mapper;
	public GetTaskPaginationQueryHandler(IMainDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<PaginationResponse<TaskDto>> Handle(GetTaskPaginationQuery request, CancellationToken cancellationToken)
	{
		var model = request.Model;
		var items = _context.Tasks
			.Include(x => x.Files)
			.Include(x => x.Status)
			.Where(x => model.From == null || x.Created >= model.From)
			.Where(x => model.To == null || x.Created <= model.To)
			.Where(x => x.Files != null && (model.WithFilesCount == null || x.Files.Count == model.WithFilesCount))
			.Where(x => model.Status == null || x.StatusId == model.Status)
			.Where(x => string.IsNullOrEmpty(model.Search) || x.Name.ToLower().Contains(model.Search.ToLower()));
			
		var sorted = items.OrderByDescending(x => x.Id);
		var dtos = (await sorted
				.Skip((model.Page - 1) * model.PageSize)
				.Take(model.PageSize)
				.ToListAsync(cancellationToken))
			.Select(x => _mapper.Map<TaskDto>(x));

		return new PaginationResponse<TaskDto>(dtos, await sorted.CountAsync(cancellationToken: cancellationToken));
	}
}