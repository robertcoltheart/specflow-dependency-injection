using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using SpecFlow.DependencyInjection;
using SpecflowDependencyInjection.Samples.WebApi;
using SpecflowDependencyInjection.Samples.WebApi.Services;

namespace SpecFlowDependencyInjection.Samples.WebApi.Tests;

internal class IntegrationApplication : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestServer();

        builder.UseEnvironment("Integration");
        
        builder.ConfigureServices(services =>
        {
            services.AddSpecFlowBindings<IntegrationApplication>();

            services.AddSingleton<IService, ProxyService>();

            services.AddRefitClient<IWeatherClient>();
        });
    }
}
