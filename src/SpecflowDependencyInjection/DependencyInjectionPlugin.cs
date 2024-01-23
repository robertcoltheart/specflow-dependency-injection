using BoDi;
using Microsoft.Extensions.DependencyInjection;
using SpecFlow.DependencyInjection;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly: RuntimePlugin(typeof(DependencyInjectionPlugin))]

namespace SpecFlow.DependencyInjection;

public class DependencyInjectionPlugin : IRuntimePlugin
{
    private static readonly object Sync = new();

    public void Initialize(
        RuntimePluginEvents runtimePluginEvents, 
        RuntimePluginParameters runtimePluginParameters, 
        UnitTestProviderConfiguration unitTestProviderConfiguration)
    {
        runtimePluginEvents.CustomizeGlobalDependencies += CustomizeGlobalDependencies;
        runtimePluginEvents.CustomizeFeatureDependencies += CustomizeFeatureDependenciesEventHandler;
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
                    var services = serviceCollectionFinder.GetServiceProvider();

                    return new RootServiceProviderContainer(services, services);
                });

                args.ObjectContainer.RegisterFactoryAs(() => args.ObjectContainer.Resolve<RootServiceProviderContainer>().ScenarioProvider);

                var events = args.ObjectContainer.Resolve<RuntimePluginTestExecutionLifecycleEvents>();
                events.AfterScenario += (_, eventArgs) => AfterEventHandler<ScenarioContext>(eventArgs.ObjectContainer);
                events.AfterFeature += (_, eventArgs) => AfterEventHandler<FeatureContext>(eventArgs.ObjectContainer);
            }

            args.ObjectContainer.Resolve<IServiceProviderFinder>();
        }
    }

    private void CustomizeFeatureDependenciesEventHandler(object? sender, CustomizeFeatureDependenciesEventArgs args)
    {
        var root = args.ObjectContainer.Resolve<RootServiceProviderContainer>();

        RegisterFactory<FeatureContext>(args.ObjectContainer, root.ScenarioProvider);
    }

    private void CustomizeScenarioDependenciesEventHandler(object? sender, CustomizeScenarioDependenciesEventArgs args)
    {
        var root = args.ObjectContainer.Resolve<RootServiceProviderContainer>();

        RegisterFactory<ScenarioContext>(args.ObjectContainer, root.ScenarioProvider);
    }

    private void AfterEventHandler<T>(IObjectContainer container)
        where T : ISpecFlowContext
    {
        if (Mappings.ActiveScopes.TryRemove(container.Resolve<T>(), out var serviceScope))
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
            Mappings.ActiveScopes.TryAdd(container.Resolve<T>(), scope);

            return scope.ServiceProvider;
        });
    }

    private class RootServiceProviderContainer
    {
        public RootServiceProviderContainer(IServiceProvider scenarioProvider, IServiceProvider featureProvider)
        {
            ScenarioProvider = scenarioProvider;
        }

        public IServiceProvider ScenarioProvider { get; }
    }
}
