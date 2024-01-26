namespace SpecFlow.DependencyInjection;

/// <summary>
/// Attribute to register a static method returning <see cref="IServiceProvider"/> for use in dependency injection
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ScenarioDependenciesAttribute : Attribute
{
}
