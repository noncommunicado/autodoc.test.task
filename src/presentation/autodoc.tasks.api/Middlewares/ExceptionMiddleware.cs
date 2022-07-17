using Serilog;

namespace autodoc.tasks.api.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next(context);
		}
		catch (HttpRequestException re)
		{
			Log.Error(re, re.Message);
			context.Response.StatusCode = (int)re.StatusCode;
			await context.Response.WriteAsJsonAsync(new
			{
				StatusCode = context.Response.StatusCode,
				Message = re.Message
			});
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Unhandled exception");
		}
	}
}