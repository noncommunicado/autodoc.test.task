using System.Net;
using autodoc.tasks.application.CQRS.Task.Commands;
using autodoc.tasks.application.CQRS.Task.Queries;
using autodoc.tasks.domain.Dto.Task;
using autodoc.tasks.domain.Http.Requests.Task;
using autodoc.tasks.domain.Http.Responses.Pagination;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace autodoc.tasks.api.Controllers;

[ApiController, Route("api/[controller]")]
public sealed class TaskController : ControllerBase
{
	private readonly IMapper _mapper;
	private readonly IMediator _mediator;
	public TaskController(IMapper mapper, IMediator mediator)
	{
		_mapper = mapper;
		_mediator = mediator;
	}

	/// <summary>
	/// Get task by Id
	/// </summary>
	[HttpGet, Route("{id}")]
	[SwaggerResponse(200, type: typeof(TaskDto))]
	[SwaggerResponse((int)HttpStatusCode.NotFound)]
	public async Task<IActionResult> Get(int id)
	{
		var entity = await _mediator.Send(new GetTaskByIdQuery(id));
		if (entity is null) return NotFound();
		return Ok(_mapper.Map<TaskDto>(entity));
	}

	[HttpGet, Route("Pagination")]
	[SwaggerResponse(200, type: typeof(PaginationResponse<TaskDto>))]
	public async Task<IActionResult> GetPagination([FromQuery] TaskPaginationRequest model)
	{
		return Ok(await _mediator.Send(new GetTaskPaginationQuery(model)));
	}

	/// <summary>
	/// Create Task with files in form (300Mb for all files limit)
	/// </summary>
	[HttpPost]
	[SwaggerResponse(200, type: typeof(int), description: "Created item Id")]
	[RequestSizeLimit(104857600 * 3)] // 300mb limit for all files
	public async Task<IActionResult> Create([FromForm] CreateTaskRequest model)
	{
		var createdId = await _mediator.Send(new CreateTaskCommand(model, model.Files));
		Log.Information($"Task #{createdId} ({model.Name}) created");
		return Ok(createdId);
	}

	/// <summary>
	/// Update Task data only
	/// </summary>
	[HttpPut]
	public async Task<IActionResult> Update([FromBody] UpdateTaskRequest model)
	{
		await _mediator.Send(new UpdateTaskCommand(model));
		Log.Information($"Task #{model.Id} updated");
		return Ok();
	}

	/// <summary>
	/// Delete task
	/// </summary>
	[HttpDelete, Route("{taskId}")]
	public async Task<IActionResult> Delete(int taskId, bool isDeleteFilesToo = false)
	{
		await _mediator.Send(new DeleteTaskCommand(taskId, isDeleteFilesToo));
		Log.Warning($"Task #{taskId} deleted");
		return Ok();
	}
}