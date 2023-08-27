using Logger.Provider.Behaviors.Interfaces;
using Logger.Provider.Shared;
using Serilog;
using Serilog.Formatting.Json;

namespace Logger.Provider.Behaviors
{
  public class LoggerConfigFileBehavior : ILoggerConfigFileBehavior
  {
    private readonly LoggerAppSettings _loggerAppSettings;

    public LoggerConfigFileBehavior(LoggerAppSettings loggerAppSettings)
    {
      _loggerAppSettings = loggerAppSettings;
    }

    public LoggerConfiguration Configuration(LoggerConfiguration serilogLoggerConfiguration)
    {
      if (_loggerAppSettings.Settings.File.Active)
      {
        serilogLoggerConfiguration.WriteTo.File(new JsonFormatter(), _loggerAppSettings.Settings.File.Path, rollingInterval: RollingInterval.Day);
      }

      return serilogLoggerConfiguration;

    }
  }
}