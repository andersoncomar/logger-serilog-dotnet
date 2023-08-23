using System.Diagnostics;
using Serilog;
using Serilog.Events;
using System.Text;
using Logger.Provider.Middlewares;
using Microsoft.AspNetCore.Builder;
using Logger.Provider.ValueObjects;
using Microsoft.Extensions.Configuration;
using Serilog.Formatting.Compact;
using Serilog.Extensions.Hosting;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

namespace Logger.Provider
{
  public static class LoggerProvider
  {
    private static LoggerSettings _loggerSettings = new LoggerSettings();
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
      builder.Host.UseSerilog();

      Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
      Serilog.Debugging.SelfLog.Enable(Console.Error);

      Log.Logger = Configuration(builder.Configuration);

      Log.Information("Starting logger");

      if (_configurationException != null)
      {
        Log.Fatal(_configurationException, "Host terminated unexpectedly");
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

                if (_loggerSettings.ShowHeader)
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

    private static ReloadableLogger Configuration(ConfigurationManager configuration)
    {
      // const string logTemplateConsole = @"[{Timestamp} {SourceContext} {Level:u4}] {Message:lj} {NewLine}{Exception} {Properties:j}";
      const string logTemplateConsole = @"[{Timestamp} {SourceContext} {Level:u4}] {Message:lj} {NewLine}{Exception}";

      _loggerSettings = configuration.GetSection(LoggerSettings.AppSettingName).Get<LoggerSettings>() ?? new LoggerSettings();

      var loggerConfig = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .Enrich.WithMachineName();

      if (_loggerSettings.Console.Active)
      {
        loggerConfig.WriteTo.Console(outputTemplate: logTemplateConsole);
      }

      try
      {
        if (_loggerSettings.File.Active)
        {
          loggerConfig.WriteTo.File(new JsonFormatter(), _loggerSettings.File.Path, rollingInterval: RollingInterval.Day);
        }

        if (_loggerSettings.Filebeat.Active)
        {
          loggerConfig.WriteTo.File(new JsonFormatter(), _loggerSettings.Filebeat.Path, rollingInterval: RollingInterval.Day);
        }

        if (_loggerSettings.Database.Active)
        {
          loggerConfig.WriteTo.MongoDB(_loggerSettings.Database.ConnectionStrings, collectionName: "LogSerilog");
        }
      }
      catch (Exception ex)
      {
        _configurationException = ex;
      }

      return loggerConfig.CreateBootstrapLogger();
    }
  }

  #endregion
}