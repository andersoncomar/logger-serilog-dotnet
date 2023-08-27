using Serilog;

namespace Logger.Provider.Behaviors.Interfaces
{
  public interface ILoggerConfigBehavior
  {
    LoggerConfiguration Configuration(LoggerConfiguration serilogLoggerConfiguration);
  }
}