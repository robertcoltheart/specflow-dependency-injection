namespace SpecFlow.DependencyInjection;

internal interface IServiceProviderFinder
{
    IServiceProvider? GetScenarioProvider();
}
