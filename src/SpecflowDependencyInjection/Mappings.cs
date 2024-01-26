using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace SpecFlow.DependencyInjection;

internal static class Mappings
{
    public static readonly ConcurrentDictionary<IServiceProvider, IContextManager> BindMappings = new();

    public static readonly ConcurrentDictionary<ISpecFlowContext, IServiceScope> ActiveServiceScopes = new();
}
