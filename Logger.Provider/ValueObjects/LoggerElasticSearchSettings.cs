namespace Logger.Provider.ValueObjects
{
  public class LoggerElasticSearchSettings : BaseChildrenSettings
  {
    public string Uri { get; set; } = "http://localhost:9200";
  }
}