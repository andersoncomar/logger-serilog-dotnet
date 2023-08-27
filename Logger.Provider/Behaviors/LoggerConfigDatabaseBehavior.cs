using Logger.Provider.Behaviors.Interfaces;
using Logger.Provider.Shared;
using Serilog;

namespace Logger.Provider.Behaviors
{
  public class LoggerConfigDatabaseBehavior : ILoggerConfigDatabaseBehavior
  {
    private readonly LoggerAppSettings _loggerAppSettings;

    public LoggerConfigDatabaseBehavior(LoggerAppSettings loggerAppSettings)
    {
      _loggerAppSettings = loggerAppSettings;
    }

    public LoggerConfiguration Configuration(LoggerConfiguration serilogLoggerConfiguration)
    {
      if (_loggerAppSettings.Settings.Database.Active)
      {
        serilogLoggerConfiguration.WriteTo.MongoDB(_loggerAppSettings.Settings.Database.ConnectionStrings, collectionName: "LogSerilog");
      }

      return serilogLoggerConfiguration;

    }
  }
}