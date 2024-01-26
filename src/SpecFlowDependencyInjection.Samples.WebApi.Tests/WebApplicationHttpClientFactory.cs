using Microsoft.AspNetCore.Mvc.Testing;

namespace SpecFlowDependencyInjection.Samples.WebApi.Tests;

internal class WebApplicationHttpClientFactory : IHttpClientFactory
{
    private readonly WebApplicationFactory<Program> factory;

    public WebApplicationHttpClientFactory(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    public HttpClient CreateClient(string name)
    {
        return factory.CreateClient();
    }
}
