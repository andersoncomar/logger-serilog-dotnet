using System.Diagnostics;
using Serilog;
using System.Text;
using Logger.Provider.Middlewares;
using Microsoft.AspNetCore.Builder;
using Logger.Provider.ValueObjects;
using Microsoft.Extensions.Configuration;
using Serilog.Extensions.Hosting;
using Elastic.Apm.NetCoreAll;
using Logger.Provider.Shared.Extensions;
using Logger.Provider.Shared.Helpers;
using Logger.Provider.Shared;
using Microsoft.Extensions.DependencyInjection;
using Logger.Provider.Factory;

namespace Logger.Provider
{
  public static class LoggerProvider
  {
    private static Exception? _configurationException = null;

    #region Methods Log

    public static void Information(string message)
    {
      Log.Information(message);
    }

    public static void Information(Exception exception, string message)
    {
      Log.Information(exception, message);
    }

    public static void Error(string message)
    {
      Log.Error(message);
    }

    public static void Error(Exception exception, string message)
    {
      Log.Error(exception, message);
    }

    public static void Fatal(Exception exception, string message)
    {
      Log.Fatal(exception, message);
    }

    public static void CloseAndFlush()
    {
      Log.CloseAndFlush();
    }

    #endregion

    #region Methods Builder / Configuration

    public static WebApplicationBuilder LoggerBuilder(this WebApplicationBuilder builder)
    {
      builder.Services.AddConfiguration(builder.Configuration);
      builder.Services.AddDependencyInjection();
      builder.Services.CreateServiceProvider();

      builder.Host.UseSerilog();

      builder.Host.VerifyIfUseApm();

      Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
      Serilog.Debugging.SelfLog.Enable(Console.Error);

      Log.Logger = BuilderLoggerConfiguration();

      Log.Information("Starting logger");

      if (_configurationException != null)
      {
        Log.Fatal(_configurationException, "Host terminated unexpectedly");
        _configurationException = null;
      }

      return builder;
    }

    public static WebApplication UseLogger(this WebApplication app)
    {
      app.UseMiddleware<SerilogRequestLogger>();

      app.UseSerilogRequestLogging(options =>
              {
                options.EnrichDiagnosticContext = (diagnosticContext, context) =>
              {
                diagnosticContext.Set("Body", String.Empty);
                diagnosticContext.Set("Headers", String.Empty);

                if (Settings.ShowHeader)
                {
                  diagnosticContext.Set("Headers", context.Request.Headers);
                }

                // Reset the request body stream position to the start so we can read it
                context.Request.Body.Position = 0;

                string body = string.Empty;

                // Leave the body open so the next middleware can read it.
                StreamReader reader = new StreamReader(
                  context.Request.Body,
                  encoding: Encoding.UTF8,
                  detectEncodingFromByteOrderMarks: false);

                body = reader.ReadToEndAsync().GetAwaiter().GetResult();

                if (string.IsNullOrEmpty(body))
                  return;

                diagnosticContext.Set("Body", body);
              };

                options.MessageTemplate = "{Headers} HTTP {RequestMethod} {RequestPath} {Body} responded {StatusCode} in {Elapsed:0.0000}";
              });

      return app;
    }

    private static void VerifyIfUseApm(this ConfigureHostBuilder host)
    {
      if (Settings.ElasticSearch.Active)
      {
        host.UseAllElasticApm();
      }
    }

    private static ReloadableLogger BuilderLoggerConfiguration()
    {
      LoggerFactory loggerFactory = ServiceManager.ServiceProvider!.GetService<LoggerFactory>()!;
      return loggerFactory.Builder();

    }

    private static LoggerSettings Settings
    {
      get
      {
        return ServiceManager.ServiceProvider!.GetService<LoggerAppSettings>()!.Settings;
      }
    }

    #endregion
  }

}