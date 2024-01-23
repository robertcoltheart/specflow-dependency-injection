using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace SpecFlow.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpecFlowBindings<TAssemblyType>(this IServiceCollection services)
    {
        return AddSpecFlowBindings(services, typeof(TAssemblyType));
    }

    public static IServiceCollection AddSpecFlowBindings(this IServiceCollection services, Type type)
    {
        var bindings = type.Assembly
            .GetTypes()
            .Where(x => x.GetCustomAttribute<BindingAttribute>() != null);

        foreach (var binding in bindings)
        {
            services.AddScoped(binding);
        }

        return services
            .AddTransient(x => Mappings.BindMappings[x].TestThreadContext)
            .AddTransient(x => Mappings.BindMappings[x].FeatureContext)
            .AddTransient(x => Mappings.BindMappings[x].ScenarioContext);
    }
}
