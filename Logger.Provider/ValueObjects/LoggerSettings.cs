namespace Logger.Provider.ValueObjects
{
  public class LoggerSettings : BaseSettings
  {
    public static new string AppSettingName = "Logger";

    public LoggerConsoleSettings Console { get; set; } = new LoggerConsoleSettings();
    public LoggerDatabaseSettings Database { get; set; } = new LoggerDatabaseSettings();
    public LoggerFileSettings File { get; set; } = new LoggerFileSettings();
    public LoggerFilebeatSettings Filebeat { get; set; } = new LoggerFilebeatSettings();
    public LoggerElasticSearchSettings ElasticSearch { get; set; } = new LoggerElasticSearchSettings();
  }
}