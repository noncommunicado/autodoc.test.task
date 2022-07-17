using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace autodoc.tasks.persistence.DbContexts.MainDb;

/// <summary>
/// Initialize Database default values
/// </summary>
public class MainDbInitializer
{
	private readonly IConfiguration _configuration;
	private readonly MainDbContext _context;

	public MainDbInitializer(
		IMainDbContext databaseContext,
		IConfiguration configuration
	)
	{
		_context = (MainDbContext)databaseContext;
		_configuration = configuration;
	}

	public async Task InitializeAsync()
	{
		await _context.Database.EnsureCreatedAsync();
		if ((_configuration.GetSection("Database:IsInitOnStartup").Get<bool?>() ?? false) is false) return;
		await InitTasksStatusesAsync();
		await InitTasksAsync();
	}

	private async Task InitTasksAsync()
	{
		if(await _context.Tasks.AnyAsync()) return;
		Random rand = new();
		for (int i = 0; i < 30; i++)
		{
			List<TaskFileEntity> files = new(5);
			for (int j = 0; j < rand.Next(0, 6); j++)
			{
				files.Add(new TaskFileEntity
				{
					FileName = $"test_{Guid.NewGuid():N}",
					FileExtension = "txt",
					LocalStorageFileId = Guid.NewGuid(),
					Size = rand.Next(10, 10000)
				});
			}

			await _context.Tasks.AddAsync(new TaskEntity
			{
				Name = $"Напоминание о встрече #{i+1}",
				Created = DateTime.Now,
				Updated = DateTime.Now,
				StatusId = rand.Next(1, 5),
				Files = files
			});
		}
		await _context.SaveChangesAsync();
	}

	public async Task InitTasksStatusesAsync()
	{
		if (await _context.TaskStatuses.AnyAsync()) return;
		
		await _context.TaskStatuses.AddRangeAsync(new TaskStatusEntity
		{
			Name = "Создано",
			EnAlias = "Created",
			Description = "Задача создана",
			Created = DateTime.Now
		}, new TaskStatusEntity
		{
			Name = "Завершено",
			EnAlias = "Completed",
			Description = "Задача была завершена",
			Created = DateTime.Now
		}, new TaskStatusEntity
		{
			Name = "В работе",
			EnAlias = "Working",
			Description = "Задача находится в работе",
			Created = DateTime.Now
		}, new TaskStatusEntity
		{
			Name = "Отменено",
			EnAlias = "Rejected",
			Description = "Задача была отменена",
			Created = DateTime.Now
		});
		await _context.SaveChangesAsync();
	}
}