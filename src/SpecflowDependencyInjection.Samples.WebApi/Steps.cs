using SpecflowDependencyInjection.Samples.WebApi.Services;
using TechTalk.SpecFlow;

namespace SpecflowDependencyInjection.Samples.WebApi;

[Binding]
public class Steps
{
    private readonly FeatureContext feature;

    private readonly ScenarioContext scenario;

    private readonly IService service;

    private readonly IWeatherClient client;

    public Steps(FeatureContext feature, ScenarioContext scenario, IService service, IWeatherClient client)
    {
        this.feature = feature;
        this.scenario = scenario;
        this.service = service;
        this.client = client;
    }

    [When("it starts")]
    public async Task WhenItStarts()
    {
        await client.GetWeatherForecast();
    }

    [Then("it should be ok")]
    public void ThenItShouldBeOk()
    {
    }
}
