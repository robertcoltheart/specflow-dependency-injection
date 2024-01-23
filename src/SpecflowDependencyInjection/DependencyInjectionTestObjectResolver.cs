using BoDi;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow.Infrastructure;

namespace SpecFlow.DependencyInjection;

internal class DependencyInjectionTestObjectResolver : ITestObjectResolver
{
    public object ResolveBindingInstance(Type bindingType, IObjectContainer container)
    {
        if (container.IsRegistered<IServiceProvider>())
        {
            var componentContext = container.Resolve<IServiceProvider>();

            return componentContext.GetRequiredService(bindingType);
        }

        return container.Resolve(bindingType);
    }
}
