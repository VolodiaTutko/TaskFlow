using NetArchTest.Rules;
using Xunit;

namespace TaskFlow.ArchitectureTests;

public class DependencyRulesTests
{
    [Fact]
    public void Domain_ShouldNotDependOn_Application()
    {
        var domainAssembly =
    typeof(TaskFlow.Domain.AssemblyReference).Assembly;

        var result = Types
            .InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskFlow.Application")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
    [Fact]
    public void Domain_ShouldNotDependOn_Infrastructure()
    {
        var assembly =
            typeof(TaskFlow.Domain.AssemblyReference).Assembly;

        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn("TaskFlow.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_ShouldNotDependOn_Infrastructure()
    {
        var assembly =
            typeof(TaskFlow.Application.DependencyInjection).Assembly;

        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn("TaskFlow.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
