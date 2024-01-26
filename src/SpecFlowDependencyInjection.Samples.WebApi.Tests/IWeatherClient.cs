using Refit;

namespace SpecFlowDependencyInjection.Samples.WebApi.Tests;

public interface IWeatherClient
{
    [Get("/weatherforecast")]
    Task GetWeatherForecast();
}
