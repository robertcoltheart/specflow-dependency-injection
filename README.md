# SpecFlow.DependencyInjection

[![NuGet](https://img.shields.io/nuget/v/SpecFlowDependencyInjection?style=for-the-badge)](https://www.nuget.org/packages/SpecFlowDependencyInjection) [![License](https://img.shields.io/github/license/robertcoltheart/specflow-dependency-injection?style=for-the-badge)](https://github.com/robertcoltheart/specflow-dependency-injection/blob/master/LICENSE)

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
                            .AddSpecFlowBindings() // Register all SpecFlow bindings classes
                            .AddSingleton<IService, ProxyService>() // Register any services that should be mocked
                            .AddHttpClient<IWeatherClient, WeatherClient>(); // Register an HTTP client
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

## License
SpecFlow.DependencyInjection is released under the [MIT License](LICENSE)
