using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using SpecFlow.DependencyInjection;
using SpecFlowDependencyInjection.Samples.WebApi.Services;
using SpecFlowDependencyInjection.Samples.WebApi.Tests.Proxies;
using SpecFlowDependencyInjection.Samples.WebApi.Tests.Support;

namespace SpecFlowDependencyInjection.Samples.WebApi.Tests;

internal class IntegrationApplication : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Integration")
            .UseTestServer()
            .ConfigureTestServices(services =>
            {
                services
                    .AddSpecFlowBindings<Hooks>()
                    .AddSingleton<IService, ProxyService>()
                    .AddSingleton<IHttpClientFactory>(new WebApplicationHttpClientFactory(this))
                    .AddRefitClient<IWeatherClient>();
            });
    }
}
