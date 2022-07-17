using System.ComponentModel.DataAnnotations;
using System.Net;
using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.application.CQRS.Task.Queries;
using autodoc.tasks.application.CQRS.TaskFile.Commands;
using autodoc.tasks.application.CQRS.TaskFile.Queries;
using autodoc.tasks.domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace autodoc.tasks.api.Controllers;

[ApiController, Route("api/[controller]")]
public sealed class TaskFileController : ControllerBase
{
	private readonly IMapper _mapper;
	private readonly IMediator _mediator;
	private readonly IArchivatorService _archivatorService;
	public TaskFileController(IMapper mapper, IMediator mediator, IArchivatorService archivatorService)
	{
		_mapper = mapper;
		_mediator = mediator;
		_archivatorService = archivatorService;
	}

	/// <summary>
	/// Upload (add) files to task
	/// </summary>
	[HttpPost, Route("Upload/ByForm/{taskId}")]
	[SwaggerResponse(200, description: "Files successfully uploaded")]
	[RequestSizeLimit(104857600 * 3)] // 300mb limit for all files
	public async Task<IActionResult> UploadFile(int taskId, [Required] List<IFormFile> files)
	{
		await _mediator.Send(new AddFilesToTaskCommand(taskId, files));
		return Ok();
	}

	/// <summary>
	/// Download single file
	/// </summary>
	[HttpGet, Route("Download/{fileId}")]
	[SwaggerResponse(200, type: typeof(FileContentResult))]
	public async Task<IActionResult> DownloadSingle(int fileId)
	{
		var fileData = await _mediator.Send(new GetTaskFileQuery(fileId));
		var provider = new FileExtensionContentTypeProvider();

		if (!provider.TryGetContentType(fileData.FileName, out var contentType))
			contentType = "application/octet-stream";

		return File(fileData.Stream, contentType, fileData.FileName);
	}

	/// <summary>
	/// Download all files in zip archive
	/// </summary>
	[HttpGet, Route("Download/Zip/{taskId}")]
	[SwaggerResponse(200, type: typeof(FileContentResult))]
	[SwaggerResponse((int)HttpStatusCode.BadRequest, description: "No files for this task found")]
	public async Task<IActionResult> DownloadAll(int taskId)
	{
		TaskEntity? task = await _mediator.Send(new GetTaskByIdQuery(taskId));

		if (task?.Files is null || task.Files.Any() is false)
			throw new HttpRequestException("No files for this task found", null, HttpStatusCode.BadRequest);

		foreach(var file in task.Files)
		{
			(FileStream Stream, string FileName) fileData;
			try {
				fileData = await _mediator.Send(new GetTaskFileQuery(file.Id));
			}
			catch (Exception ex) {
				Log.Error(ex, "Task files wasn't find");
				continue;
			}
			await _archivatorService.AddFileAsync(fileData.FileName, fileData.Stream);
		};

		var bytes = _archivatorService.GetArchiveBytesAndCloseArchive();
		string taskSubstring = task.Name.Length > 20 ? $"{task.Name.Substring(0, 19)}..." : task.Name;
		string fileName = $"Файлы задачи ({taskSubstring}).zip";
		return new FileContentResult(bytes, "application/zip")
		{
			FileDownloadName = fileName
		};
	}

	[HttpDelete, Route("{fileId}")]
	[SwaggerResponse(200, description: "Deleted successfully")]
	[SwaggerResponse((int)HttpStatusCode.NotFound, description: "Task file not found")]
	public async Task<IActionResult> Delete(int fileId)
	{
		await _mediator.Send(new DeleteTaskFileCommand(fileId));
		return Ok();
	}
}