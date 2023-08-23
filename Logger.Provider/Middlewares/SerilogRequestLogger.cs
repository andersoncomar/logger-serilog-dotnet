using Microsoft.AspNetCore.Http;

namespace Logger.Provider.Middlewares
{
  public class SerilogRequestLogger
  {
    readonly RequestDelegate _next;

    public SerilogRequestLogger(RequestDelegate next)
    {
      if (next == null) throw new ArgumentNullException(nameof(next));
      _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
      httpContext.Request.EnableBuffering();

      await _next(httpContext);

      httpContext.Request.Body.Position = 0;
    }
  }
}

