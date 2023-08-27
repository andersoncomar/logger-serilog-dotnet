using Logger.Provider.Behaviors.Interfaces;
using Logger.Provider.Shared;
using Serilog;
using Serilog.Formatting.Json;

namespace Logger.Provider.Behaviors
{
  public class LoggerConfigFilebeatBehavior : ILoggerConfigFilebeatBehavior
  {
    private readonly LoggerAppSettings _loggerAppSettings;

    public LoggerConfigFilebeatBehavior(LoggerAppSettings loggerAppSettings)
    {
      _loggerAppSettings = loggerAppSettings;
    }

    public LoggerConfiguration Configuration(LoggerConfiguration serilogLoggerConfiguration)
    {
      if (_loggerAppSettings.Settings.Filebeat.Active)
      {
        serilogLoggerConfiguration.WriteTo.File(new JsonFormatter(), _loggerAppSettings.Settings.Filebeat.Path, rollingInterval: RollingInterval.Day);
      }

      return serilogLoggerConfiguration;

    }
  }
}