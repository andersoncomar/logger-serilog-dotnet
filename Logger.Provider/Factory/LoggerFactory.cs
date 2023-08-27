using Logger.Provider.Behaviors.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Extensions.Hosting;

namespace Logger.Provider.Factory
{
  public class LoggerFactory
  {
    private readonly ILoggerConfigConsoleBehavior _loggerConfigConsoleBehavior;
    private readonly ILoggerConfigFileBehavior _loggerConfigFileBehavior;
    private readonly ILoggerConfigFilebeatBehavior _loggerConfigFilebeatBehavior;
    private readonly ILoggerConfigElasticSearchBehavior _loggerConfigElasticSearchBehavior;
    private readonly ILoggerConfigDatabaseBehavior _loggerConfigDatabaseBehavior;

    public LoggerFactory(
      ILoggerConfigConsoleBehavior loggerConfigConsoleBehavior,
      ILoggerConfigFileBehavior loggerConfigFileBehavior,
      ILoggerConfigFilebeatBehavior loggerConfigFilebeatBehavior,
      ILoggerConfigElasticSearchBehavior loggerConfigElasticSearchBehavior,
      ILoggerConfigDatabaseBehavior loggerConfigDatabaseBehavior)
    {
      _loggerConfigConsoleBehavior = loggerConfigConsoleBehavior;
      _loggerConfigFileBehavior = loggerConfigFileBehavior;
      _loggerConfigFilebeatBehavior = loggerConfigFilebeatBehavior;
      _loggerConfigElasticSearchBehavior = loggerConfigElasticSearchBehavior;
      _loggerConfigDatabaseBehavior = loggerConfigDatabaseBehavior;
    }

    public ReloadableLogger Builder()
    {
      LoggerConfiguration loggerConfig = BuilderBase();

      loggerConfig = _loggerConfigConsoleBehavior.Configuration(loggerConfig);
      loggerConfig = _loggerConfigFileBehavior.Configuration(loggerConfig);
      loggerConfig = _loggerConfigFilebeatBehavior.Configuration(loggerConfig);
      loggerConfig = _loggerConfigElasticSearchBehavior.Configuration(loggerConfig);
      loggerConfig = _loggerConfigDatabaseBehavior.Configuration(loggerConfig);

      return loggerConfig.CreateBootstrapLogger();
    }

    private static LoggerConfiguration BuilderBase()
    {
      return new LoggerConfiguration()
          .MinimumLevel.Debug()
          .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
          .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
          .MinimumLevel.Override("Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware", LogEventLevel.Information)
          .Enrich.FromLogContext()
          .Enrich.WithExceptionDetails()
          .Enrich.WithMachineName();
    }
  }
}