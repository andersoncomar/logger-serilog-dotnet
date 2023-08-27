using Logger.Provider.Behaviors;
using Logger.Provider.Behaviors.Interfaces;
using Logger.Provider.Factory;
using Logger.Provider.Shared.Helpers;
using Logger.Provider.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logger.Provider.Shared.Extensions
{
  public static class ServiceExtensions
  {
    public static IServiceCollection AddConfiguration(
                 this IServiceCollection services, IConfiguration config)
    {
      services.Configure<LoggerSettings>(
          config.GetSection(LoggerSettings.AppSettingName));

      return services;
    }

    public static IServiceCollection AddDependencyInjection(
             this IServiceCollection services)
    {
      services.AddSingleton<LoggerAppSettings>();

      services.AddScoped<LoggerFactory>();
      services.AddScoped<ILoggerConfigConsoleBehavior, LoggerConfigConsoleBehavior>();
      services.AddScoped<ILoggerConfigFileBehavior, LoggerConfigFileBehavior>();
      services.AddScoped<ILoggerConfigFilebeatBehavior, LoggerConfigFilebeatBehavior>();
      services.AddScoped<ILoggerConfigElasticSearchBehavior, LoggerConfigElasticSearchBehavior>();
      services.AddScoped<ILoggerConfigDatabaseBehavior, LoggerConfigDatabaseBehavior>();

      return services;
    }

    public static IServiceProvider CreateServiceProvider(
         this IServiceCollection services)
    {
      ServiceManager.ServiceProvider = services.BuildServiceProvider();

      return ServiceManager.ServiceProvider;
    }
  }
}