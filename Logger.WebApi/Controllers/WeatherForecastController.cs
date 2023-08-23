using Logger.Provider;
using Microsoft.AspNetCore.Mvc;

namespace Logger.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("GetWithError", Name = "GetWithError")]
    public void GetWithError()
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (System.Exception exception)
        {
            LoggerProvider.Error(exception, "Hello World");
        }
    }

    [HttpPost()]
    public void PostInfoRequest(FromBodyAttribute body)
    {
        // var exampleUser = new { Id = "1001", UserName = "Adam", SecurityStamp = DateTime.Now.ToString() };
        // _logger.Information("Created {@User} on {Created}", exampleUser, DateTime.Now);
    }
}
