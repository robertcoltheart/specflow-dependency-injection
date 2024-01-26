using System.Reflection;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace SpecFlow.DependencyInjection;

internal static class BindingRegistryExtensions
{
    public static IEnumerable<Assembly> GetBindingAssemblies(this IBindingRegistry bindingRegistry)
    {
        return bindingRegistry
            .GetBindingTypes()
            .OfType<RuntimeBindingType>()
            .Select(t => t.Type.Assembly)
            .Distinct();
    }

    private static IEnumerable<IBindingType> GetBindingTypes(this IBindingRegistry bindingRegistry)
    {
        return bindingRegistry
            .GetStepDefinitions()
            .Cast<IBinding>()
            .Concat(bindingRegistry.GetHooks())
            .Concat(bindingRegistry.GetStepTransformations())
            .Select(b => b.Method.Type)
            .Distinct();
    }
}
