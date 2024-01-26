using BoDi;
using Microsoft.Extensions.DependencyInjection;
using SpecFlow.DependencyInjection;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly: RuntimePlugin(typeof(DependencyInjectionPlugin))]

namespace SpecFlow.DependencyInjection;

internal class DependencyInjectionPlugin : IRuntimePlugin
{
    private static readonly object Sync = new();

    public void Initialize(
        RuntimePluginEvents runtimePluginEvents, 
        RuntimePluginParameters runtimePluginParameters, 
        UnitTestProviderConfiguration unitTestProviderConfiguration)
    {
        runtimePluginEvents.CustomizeGlobalDependencies += CustomizeGlobalDependencies;
        runtimePluginEvents.CustomizeScenarioDependencies += CustomizeScenarioDependenciesEventHandler;
    }

    private void CustomizeGlobalDependencies(object? sender, CustomizeGlobalDependenciesEventArgs args)
    {
        if (!args.ObjectContainer.IsRegistered<IServiceProviderFinder>())
        {
            lock (Sync)
            {
                if (!args.ObjectContainer.IsRegistered<IServiceProviderFinder>())
                {
                    args.ObjectContainer.RegisterTypeAs<DependencyInjectionTestObjectResolver, ITestObjectResolver>();
                    args.ObjectContainer.RegisterTypeAs<ServiceProviderFinder, IServiceProviderFinder>();
                }

                args.ObjectContainer.RegisterFactoryAs(() =>
                {
                    var serviceCollectionFinder = args.ObjectContainer.Resolve<IServiceProviderFinder>();

                    var scenarioProvider = serviceCollectionFinder.GetScenarioProvider();

                    if (scenarioProvider == null)
                    {
                        throw new InvalidOperationException("Unable to find scenario dependencies. Mark a static method that returns IServiceProvider with [ScenarioDependencies].");
                    }

                    return new RootServiceProviderContainer(scenarioProvider);
                });

                args.ObjectContainer.RegisterFactoryAs(() => args.ObjectContainer.Resolve<RootServiceProviderContainer>().ScenarioProvider);

                var events = args.ObjectContainer.Resolve<RuntimePluginTestExecutionLifecycleEvents>();
                events.AfterScenario += (_, eventArgs) => AfterEventHandler<ScenarioContext>(eventArgs.ObjectContainer);
                events.AfterFeature += (_, eventArgs) => AfterEventHandler<FeatureContext>(eventArgs.ObjectContainer);
            }

            args.ObjectContainer.Resolve<IServiceProviderFinder>();
        }
    }

    private void CustomizeScenarioDependenciesEventHandler(object? sender, CustomizeScenarioDependenciesEventArgs args)
    {
        var root = args.ObjectContainer.Resolve<RootServiceProviderContainer>();

        RegisterFactory<ScenarioContext>(args.ObjectContainer, root.ScenarioProvider);
    }

    private void AfterEventHandler<T>(IObjectContainer container)
        where T : ISpecFlowContext
    {
        if (Mappings.ActiveServiceScopes.TryRemove(container.Resolve<T>(), out var serviceScope))
        {
            Mappings.BindMappings.TryRemove(serviceScope.ServiceProvider, out _);
            serviceScope.Dispose();
        }
    }

    private void RegisterFactory<T>(IObjectContainer container, IServiceProvider services)
        where T : ISpecFlowContext
    {
        container.RegisterFactoryAs(_ =>
        {
            var scope = services.CreateScope();

            Mappings.BindMappings.TryAdd(scope.ServiceProvider, container.Resolve<IContextManager>());
            Mappings.ActiveServiceScopes.TryAdd(container.Resolve<T>(), scope);

            return scope.ServiceProvider;
        });
    }

    private class RootServiceProviderContainer
    {
        public RootServiceProviderContainer(IServiceProvider scenarioProvider)
        {
            ScenarioProvider = scenarioProvider;
        }

        public IServiceProvider ScenarioProvider { get; }
    }
}
