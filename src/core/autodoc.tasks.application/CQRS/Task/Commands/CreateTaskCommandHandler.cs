using System.Collections.Concurrent;
using System.Net;
using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.application.CQRS.TaskStatus.Commands;
using autodoc.tasks.domain.Entities;
using autodoc.tasks.domain.Http.Requests.Task;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace autodoc.tasks.application.CQRS.Task.Commands;

public sealed record CreateTaskCommand(CreateTaskRequest Model, IEnumerable<IFormFile>? Files = null) : IRequest<int>;

public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, int>
{
	private readonly IMainDbContext _context;
	private readonly ITaskFileManager _fileManager;
	public CreateTaskCommandHandler(IMainDbContext context, ITaskFileManager fileManager)
	{
		_context = context;
		_fileManager = fileManager;
	}

	public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
	{
		if (request.Model.StatusId.HasValue)
			if (await new IsTaskStatusExistCommandHandler(_context)
				    .Handle(new IsTaskStatusExistCommand(request.Model.StatusId.Value), cancellationToken) is false)
				throw new HttpRequestException("Task status not found", null, HttpStatusCode.BadRequest);

		BlockingCollection<TaskFileEntity> savedFiles = new();
		if (request.Files != null && request.Files.Any())
		{
			await Parallel.ForEachAsync(request.Files, cancellationToken, async (file, token) =>
			{
				if (file.Length <= 0) return;
				Guid localStorageFileId = Guid.NewGuid();
				await _fileManager.SaveFileAsync(file, localStorageFileId.ToString("N"), token);
				savedFiles.Add(new TaskFileEntity
				{
					Created = DateTime.Now,
					FileExtension = Path.GetExtension(file.FileName).Replace(".", string.Empty),
					FileName = Path.GetFileNameWithoutExtension(file.FileName),
					LocalStorageFileId = localStorageFileId,
					Size = file.Length
				}, token);
			});
		}

		var entity = new TaskEntity
		{
			Name = request.Model.Name,
			StatusId = request.Model.StatusId,
			Files = savedFiles.ToArray(),
			Created = DateTime.Now
		};
		await _context.Tasks.AddAsync(entity, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return entity.Id;
	}
}