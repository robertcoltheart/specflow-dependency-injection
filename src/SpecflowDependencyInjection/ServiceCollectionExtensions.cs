using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace SpecFlow.DependencyInjection;

/// <summary>
/// Service collection extensions for SpecFlow
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add SpecFlow bindings from the calling assembly
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddSpecFlowBindings(this IServiceCollection services)
    {
        return services.AddSpecFlowBindings(Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// Add SpecFlow bindings from the assembly containing the given type
    /// </summary>
    /// <typeparam name="TAssemblyType">The type in the assembly to register</typeparam>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddSpecFlowBindings<TAssemblyType>(this IServiceCollection services)
    {
        return AddSpecFlowBindings(services, typeof(TAssemblyType));
    }

    /// <summary>
    /// Add SpecFlow bindings from the assembly containing the given type
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="type">The type in the assembly to register</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddSpecFlowBindings(this IServiceCollection services, Type type)
    {
        return services.AddSpecFlowBindings(type.Assembly);
    }

    /// <summary>
    /// Add SpecFlow bindings from the given assembly
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assembly">The assembly to register</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddSpecFlowBindings(this IServiceCollection services, Assembly assembly)
    {
        var bindings = assembly
            .GetTypes()
            .Where(x => x.GetCustomAttribute<BindingAttribute>() != null);

        foreach (var binding in bindings)
        {
            services.AddScoped(binding);
        }

        return services
            .AddTransient(provider => Mappings.BindMappings[provider].TestThreadContext)
            .AddTransient(provider => Mappings.BindMappings[provider].FeatureContext)
            .AddTransient(provider => Mappings.BindMappings[provider].ScenarioContext);
    }
}
