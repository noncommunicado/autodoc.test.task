using System.ComponentModel.DataAnnotations;
using autodoc.tasks.application.CQRS.TaskStatus.Commands;
using autodoc.tasks.application.CQRS.TaskStatus.Queries;
using autodoc.tasks.domain.Dto.TaskStatus;
using autodoc.tasks.domain.Http.Requests.TaskStatus;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace autodoc.tasks.api.Controllers;

[ApiController, Route("api/[controller]")]
public sealed class TaskStatusController : ControllerBase
{
	private readonly IMapper _mapper;
	private readonly IMediator _mediator;
	public TaskStatusController(IMapper mapper, IMediator mediator)
	{
		_mapper = mapper;
		_mediator = mediator;
	}

	/// <summary>
	/// Get all task statuses
	/// </summary>
	/// <param name="search">search filter param</param>
	[HttpGet, Route("All")]
	[SwaggerResponse(200, type: typeof(IEnumerable<TaskStatusDto>))]
	public async Task<IActionResult> GetAll([MaxLength(20)] string? search = null)
	{
		var entities = await _mediator.Send(new GetAllTaskStatusesQuery(search));
		return Ok(entities.Select(x => _mapper.Map<TaskStatusDto>(x)));
	}

	/// <summary>
	/// Get task status with description
	/// </summary>
	[HttpGet, Route("{id}")]
	[SwaggerResponse(200, type: typeof(TaskStatusExtendedDto))]
	public async Task<IActionResult> Get(int id)
	{
		var entity = await _mediator.Send(new GetTaskStatusByIdQuery(id));
		if (entity is null) return NotFound();
		return Ok(_mapper.Map<TaskStatusExtendedDto>(entity));
	}

	/// <summary>
	/// Creates new Task Status, returns created Id
	/// </summary>
	[HttpPost]
	[SwaggerResponse(200, type: typeof(int), description: "Returns Id of created element")]
	[SwaggerResponse(409, description: "Status with same name already exist")]
	public async Task<IActionResult> Create([FromBody] CreateTaskStatusRequest model)
	{
		int createdId = await _mediator.Send(new CreateTaskStatusCommand(model));
		return Ok(createdId);
	}

	/// <summary>
	/// Update record
	/// </summary>
	[HttpPut]
	[SwaggerResponse(200, description: "Success")]
	public async Task<IActionResult> Update([FromBody] UpdateTaskStatusRequest model)
	{
		await _mediator.Send(new UpdateTaskStatusCommand(model));
		return Ok();
	}

	/// <summary>
	/// Delete record
	/// </summary>
	[HttpDelete, Route("{id}")]
	[SwaggerResponse(200, description: "Success")]
	[SwaggerResponse(409, description: "Status with same name already exist")]
	public async Task<IActionResult> Delete(int id)
	{
		await _mediator.Send(new DeleteTaskStatusCommand(id));
		return Ok();
	}
}