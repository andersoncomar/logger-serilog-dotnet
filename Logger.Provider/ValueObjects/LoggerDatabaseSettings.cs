namespace Logger.Provider.ValueObjects
{
  public class LoggerDatabaseSettings : BaseChildrenSettings
  {
    public string ConnectionStrings { get; set; } = string.Empty;
  }
}