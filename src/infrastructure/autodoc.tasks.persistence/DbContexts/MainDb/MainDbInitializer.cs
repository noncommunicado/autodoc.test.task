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