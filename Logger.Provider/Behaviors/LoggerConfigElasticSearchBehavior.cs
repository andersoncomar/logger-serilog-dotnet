using Elastic.Apm.SerilogEnricher;
using Elastic.Serilog.Sinks;
using Logger.Provider.Behaviors.Interfaces;
using Logger.Provider.Shared;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Logger.Provider.Behaviors
{
  public class LoggerConfigElasticSearchBehavior : ILoggerConfigElasticSearchBehavior
  {
    private readonly LoggerAppSettings _loggerAppSettings;

    public LoggerConfigElasticSearchBehavior(LoggerAppSettings loggerAppSettings)
    {
      _loggerAppSettings = loggerAppSettings;
    }

    public LoggerConfiguration Configuration(LoggerConfiguration serilogLoggerConfiguration)
    {
      if (_loggerAppSettings.Settings.ElasticSearch.Active)
      {
        const string logTemplateElasticSearch = @"[{ElasticApmTraceId} {ElasticApmTransactionId} {ElasticApmSpanId} {Message:lj} {NewLine}{Exception}";

        serilogLoggerConfiguration.Enrich.WithElasticApmCorrelationInfo();
        serilogLoggerConfiguration.WriteTo.Console(outputTemplate: logTemplateElasticSearch);
        serilogLoggerConfiguration.WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(_loggerAppSettings.Settings.ElasticSearch.Uri))
        {
          DetectElasticsearchVersion = false,
          AutoRegisterTemplate = true,
          AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
          ModifyConnectionSettings = configuration => configuration.ServerCertificateValidationCallback(
                      (o, certificate, arg3, arg4) => { return true; })
        });
      }

      return serilogLoggerConfiguration;

    }
  }
}