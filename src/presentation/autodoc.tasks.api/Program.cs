using System.Reflection;
using autodoc.tasks.api.Middlewares;
using autodoc.tasks.application.Common.Interfaces;
using autodoc.tasks.application.CQRS.TaskStatus.Commands;
using autodoc.tasks.application.Mappings;
using autodoc.tasks.persistence;
using autodoc.tasks.persistence.DbContexts.MainDb;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;

// Logging
Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Logger(c =>
		c.WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "_.log"),
			rollingInterval: RollingInterval.Day,
			restrictedToMinimumLevel: LogEventLevel.Information))
	.CreateLogger();

Log.Information("Application starting...");
var builder = WebApplication.CreateBuilder(args);

#region Configuration setUp for Linux/Unix hosting only
//builder.Configuration.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
//builder.Configuration.AddJsonFile("appsettings.json", false, false); 
#endregion

// Configure Services
{
	builder.Services.AddScoped<ExceptionMiddleware>();
	builder.Services.AddControllers();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen(x =>
	{
		x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"), true);
	});

	// Inject persistence services
	builder.Services.AddPersistence(builder.Configuration);

	// Add auto-mapper profiles
	builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(Program)),
		Assembly.GetAssembly(typeof(autodoc.tasks.application.AssemblyInfo)));

	// Add CQRS via MediatR
	builder.Services.AddMediatR(Assembly.GetAssembly(typeof(Program))!,
		Assembly.GetAssembly(typeof(autodoc.tasks.application.AssemblyInfo))!);
}

var app = builder.Build();

if (builder.Configuration.GetSection("Swagger:IsEnabled").Get<bool?>() ?? false)
{
	Log.Information("Also running Swagger...");
	app.UseSwagger();
	app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Autodoc tasks API"));
}

// For Cors issues
app.UseCors(x =>
{
	x.SetIsOriginAllowed(s => true)
		.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials();
});

// For nginx hosting
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Listen to
foreach (var host in builder.Configuration.GetSection("Network:ListenTo").Get<string[]>())
{
	app.Urls.Add(host);
	Log.Information("Become to listen on {host}", host);
}

// Init Database
using (var scope = app.Services.CreateScope())
{
	await new MainDbInitializer(
			scope.ServiceProvider.GetService<IMainDbContext>()!,
			scope.ServiceProvider.GetService<ITaskFileManager>()!,
			app.Configuration)
		.InitializeAsync();
}

Log.Information("Application started");
app.Run();
