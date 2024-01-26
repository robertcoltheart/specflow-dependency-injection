using SpecFlowDependencyInjection.Samples.WebApi.Services;
using SpecFlowDependencyInjection.Samples.WebApi.Tests.Proxies;
using TechTalk.SpecFlow;
using Xunit;

namespace SpecFlowDependencyInjection.Samples.WebApi.Tests.Steps;

[Binding]
public class FeatureSteps
{
    private readonly FeatureContext feature;

    private readonly ScenarioContext scenario;

    private readonly IService service;

    private readonly IWeatherClient client;

    public FeatureSteps(FeatureContext feature, ScenarioContext scenario, IService service, IWeatherClient client)
    {
        this.feature = feature;
        this.scenario = scenario;
        this.service = service;
        this.client = client;
    }

    [When("it starts")]
    public async Task WhenItStarts()
    {
        feature["key"] = "value1";
        scenario["key"] = "value2";

        await client.GetWeatherForecast();
    }

    [Then("it should be ok")]
    public void ThenItShouldBeOk()
    {
        Assert.Equal(nameof(ProxyService), service.GetType().Name);
    }
}
