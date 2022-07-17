using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace autodoc.tasks.api.Controllers;

[ApiController, Route("api/[controller]")]
public sealed class TaskFileController : ControllerBase
{
	private readonly IMapper _mapper;
	private readonly IMediator _mediator;
	public TaskFileController(IMapper mapper, IMediator mediator)
	{
		_mapper = mapper;
		_mediator = mediator;
	}

	/// <summary>
	/// Upload files to task
	/// </summary>
	/// <param name="taskId"></param>
	/// <returns></returns>
	[HttpPost, Route("Upload/ByForm/{taskId}")]
	[SwaggerResponse(200, description: "Files successfully uploaded")]
	[SwaggerResponse(400, description: "No files presented in form")]
	[RequestSizeLimit(104857600)] // 100mb
	public async Task<IActionResult> UploadFile(int taskId, List<IFormFile> files)
	{
		if (Request.Form.Files.Any() is false)
			return BadRequest("No files presented in form");

		return Ok();
	}

	/// <summary>
	/// Download single file
	/// </summary>
	[HttpGet, Route("Download/{fileId}")]
	[SwaggerResponse(200, type: typeof(FileContentResult))]
	public async Task<IActionResult> DownloadSingle(int fileId)
	{
		//return File(,);
		return Ok();

		//return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sheetName + ".xlsx");
	}
}