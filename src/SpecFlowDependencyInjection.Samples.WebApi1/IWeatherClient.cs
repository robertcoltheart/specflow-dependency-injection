using Refit;

namespace SpecFlowDependencyInjection.Samples.WebApi;

public interface IWeatherClient
{
    [Get("/weatherforecast")]
    Task GetWeatherForecast();
}
