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

    public IServiceProvider? GetScenarioProvider()
    {
        return GetServiceProvider<ScenarioDependenciesAttribute>();
    }

    private IServiceProvider? GetServiceProvider<T>()
        where T : Attribute
    {
        var method = GetMethods()
            .FirstOrDefault(x => x.GetCustomAttribute<T>() != null);

        if (method == null)
        {
            return null;
        }

        return GetServiceProvider(method);
    }

    private IEnumerable<MethodInfo> GetMethods()
    {
        return bindingRegistry
            .GetBindingAssemblies()
            .SelectMany(x => x.GetTypes())
            .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            .Where(x => x.CustomAttributes.Any());
    }

    private static IServiceProvider? GetServiceProvider(MethodInfo method)
    {
        return method.Invoke(null, null) as IServiceProvider;
    }
}
