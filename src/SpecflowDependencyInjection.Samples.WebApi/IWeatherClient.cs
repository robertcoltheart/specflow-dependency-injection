using Refit;

namespace SpecflowDependencyInjection.Samples.WebApi;

public interface IWeatherClient
{
    [Get("/weatherforecast")]
    Task GetWeatherForecast();
}
