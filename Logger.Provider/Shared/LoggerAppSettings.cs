using Logger.Provider.ValueObjects;
using Microsoft.Extensions.Options;

namespace Logger.Provider.Shared
{
  public class LoggerAppSettings
  {
    private readonly LoggerSettings _loggerSettings;
    public LoggerAppSettings(IOptions<LoggerSettings> loggerSettings)
    {
      _loggerSettings = loggerSettings.Value;
    }

    public LoggerSettings Settings { get { return _loggerSettings; } }
  }
}