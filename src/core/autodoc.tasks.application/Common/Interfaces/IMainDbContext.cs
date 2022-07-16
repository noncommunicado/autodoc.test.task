using autodoc.tasks.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace autodoc.tasks.application.Common.Interfaces;

public interface IMainDbContext
{
	DbSet<TaskEntity> Tasks { get; set; }
	DbSet<TaskStatusEntity> TaskStatuses { get; set; }
	DbSet<TaskFileEntity> TaskFiles { get; set; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}