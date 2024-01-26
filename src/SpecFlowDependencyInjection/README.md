## About

A SpecFlow plugin that uses `Microsoft.Extensions.DependencyInjection` as a DI container.

## Usage
Install the package from NuGet with `dotnet add package SpecFlowDependencyInjection`.

You can expose a static method decorated with `[ScenarioDependencies]` that returns an `IServiceProvider`.

For testing an ASP.NET Core application, configure a hooks class as below:

```csharp
[Binding]
public class Hooks
{
    private static WebApplicationFactory<Program>? application;

    [ScenarioDependencies]
    public static IServiceProvider GetServices()
    {
        return application!.Services;
    }

    [BeforeTestRun]
    public static void BeforeRun()
    {
        application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder
                    .UseTestServer()
                    .ConfigureTestServices(services =>
                    {
                        services
                            // Register all SpecFlow bindings classes
                            .AddSpecFlowBindings()
                            // Register any services that should be mocked
                            .AddSingleton<IService, ProxyService>()
                            // Register an HTTP client
                            .AddHttpClient<IWeatherClient, WeatherClient>();
                    });
            });
    }

    [AfterTestRun]
    public static void AfterRun()
    {
        application?.Dispose();
    }
}
```

## Get in touch
Raise an [issue](https://github.com/robertcoltheart/specflow-dependency-injection/issues).

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute to this project.
