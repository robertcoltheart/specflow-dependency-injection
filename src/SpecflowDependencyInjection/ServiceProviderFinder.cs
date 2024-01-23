using System.Reflection;
using TechTalk.SpecFlow.Bindings;

namespace SpecFlow.DependencyInjection;

internal class ServiceProviderFinder : IServiceProviderFinder
{
    private readonly IBindingRegistry bindingRegistry;

    public ServiceProviderFinder(IBindingRegistry bindingRegistry)
    {
        this.bindingRegistry = bindingRegistry;
    }

    public IServiceProvider GetServiceProvider()
    {
        var methods = bindingRegistry
            .GetBindingAssemblies()
            .SelectMany(x => x.GetTypes())
            .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            .Where(x => x.CustomAttributes.Any())
            .ToArray();

        var scenarioMethod = methods
            .FirstOrDefault(x => x.GetCustomAttribute<ScenarioDependenciesAttribute>() != null);

        var featureMethod = methods
            .FirstOrDefault(x => x.GetCustomAttribute<FeatureDependenciesAttribute>() != null);

        throw new Exception("Unable to find scenario dependencies. Mark a static method that returns IServiceProvider with [ScenarioDependencies].");
    }

    private static IServiceProvider? GetServiceProvider(MethodInfo method)
    {
        return method.Invoke(null, null) as IServiceProvider;
    }
}
