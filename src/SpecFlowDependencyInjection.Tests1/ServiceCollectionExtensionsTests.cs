using Microsoft.Extensions.DependencyInjection;
using SpecFlow.DependencyInjection;
using TechTalk.SpecFlow;
using Xunit;

namespace SpecFlowDependencyInjection.Tests;

[Binding]
public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void CanRegisterAssembly()
    {
        var services = new ServiceCollection()
            .AddSpecFlowBindings(typeof(ServiceCollectionExtensionsTests).Assembly);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ServiceCollectionExtensionsTests));

        Assert.NotNull(descriptor);
    }

    [Fact]
    public void CanRegisterCallingAssembly()
    {
        var services = new ServiceCollection()
            .AddSpecFlowBindings();

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ServiceCollectionExtensionsTests));

        Assert.NotNull(descriptor);
    }

    [Fact]
    public void CanRegisterType()
    {
        var services = new ServiceCollection()
            .AddSpecFlowBindings(typeof(ServiceCollectionExtensionsTests));

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ServiceCollectionExtensionsTests));

        Assert.NotNull(descriptor);
    }

    [Fact]
    public void CanRegisterTyped()
    {
        var services = new ServiceCollection()
            .AddSpecFlowBindings<ServiceCollectionExtensionsTests>();

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ServiceCollectionExtensionsTests));

        Assert.NotNull(descriptor);
    }
}
