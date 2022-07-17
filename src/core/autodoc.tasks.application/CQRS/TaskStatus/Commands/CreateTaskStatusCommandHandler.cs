using System.Net;
using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.domain.Dto.TaskStatus;
using autodoc.tasks.domain.Entities;
using autodoc.tasks.domain.Http.Requests.TaskStatus;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.CQRS.TaskStatus.Commands;

public sealed record CreateTaskStatusCommand : IRequest<int>
{
	public CreateTaskStatusRequest Model { get; }
	public CreateTaskStatusCommand(CreateTaskStatusRequest model)
	{
		Model = model;
	}
}

public sealed class CreateTaskStatusCommandHandler : IRequestHandler<CreateTaskStatusCommand, int>
{
	private readonly IMainDbContext _context;
	private readonly IMapper _mapper;
	public CreateTaskStatusCommandHandler(IMainDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<int> Handle(CreateTaskStatusCommand request, CancellationToken cancellationToken)
	{
		var entity = _mapper.Map<TaskStatusEntity>(request.Model);

		if (_context.TaskStatuses.AsNoTrackingWithIdentityResolution()
		    .Any(x => x.Name.ToLower() == request.Model.Name.ToLower()))
			throw new HttpRequestException("Status with same name already exist", null, HttpStatusCode.Conflict);

		await _context.TaskStatuses.AddAsync(entity, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return entity.Id;
	}
}