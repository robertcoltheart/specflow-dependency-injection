using SpecFlow.DependencyInjection;
using TechTalk.SpecFlow;

namespace SpecFlowDependencyInjection.Samples.WebApi.Tests;

[Binding]
public class Hooks
{
    private static IntegrationApplication? application;

    [ScenarioDependencies]
    public static IServiceProvider GetServices()
    {
        return application!.Services;
    }

    [BeforeTestRun]
    public static void BeforeRun()
    {
        application = new IntegrationApplication();

        var client = application.CreateClient();
    }

    [AfterTestRun]
    public static void AfterRun()
    {
        application?.Dispose();
    }
}
