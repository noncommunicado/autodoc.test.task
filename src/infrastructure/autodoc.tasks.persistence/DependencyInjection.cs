using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.persistence.DbContexts.MainDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace autodoc.tasks.persistence;

public static class DependencyInjection
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<IMainDbContext, MainDbContext>(x => x.UseSqlServer(
			configuration.GetConnectionString("Main")));
		return services;
	}
}