using Logger.Provider.Behaviors.Interfaces;
using Logger.Provider.Shared;
using Serilog;

namespace Logger.Provider.Behaviors
{
  public class LoggerConfigConsoleBehavior : ILoggerConfigConsoleBehavior
  {
    private readonly LoggerAppSettings _loggerAppSettings;

    public LoggerConfigConsoleBehavior(LoggerAppSettings loggerAppSettings)
    {
      _loggerAppSettings = loggerAppSettings;
    }

    public LoggerConfiguration Configuration(LoggerConfiguration serilogLoggerConfiguration)
    {
      const string logTemplateConsole = @"[{Timestamp} {SourceContext} {Level:u4}] {Message:lj} {NewLine}{Exception}";

      if (_loggerAppSettings.Settings.Console.Active && !_loggerAppSettings.Settings.ElasticSearch.Active)
      {
        serilogLoggerConfiguration.WriteTo.Console(outputTemplate: logTemplateConsole);
      }

      return serilogLoggerConfiguration;

    }
  }
}