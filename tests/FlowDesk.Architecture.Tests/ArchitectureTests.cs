using FluentAssertions;
using NetArchTest.Rules;

namespace FlowDesk.Architecture.Tests;

public class ArchitectureTests
{
    private const string DomainNamespace = "FlowDesk.Domain";
    private const string ApplicationNamespace = "FlowDesk.Application";
    private const string InfrastructureNamespace = "FlowDesk.Infrastructure";
    private const string ApiNamespace = "FlowDesk.Api";

    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Layers()
    {
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
            [
                ApplicationNamespace,
                InfrastructureNamespace,
                ApiNamespace
            ])
            .GetResult();
        
        var failingTypes = result.FailingTypes?.Select(t => t.FullName) ?? [];

        failingTypes.Should().BeNullOrEmpty("Domain must not depend on Application, Infrastructure, or API layers.");
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_EntityFrameworkCore()
    {
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore")
            .GetResult();

        var failingTypes = result.FailingTypes?.Select(t => t.FullName) ?? [];
        failingTypes.Should().BeNullOrEmpty("Domain entities and business rules must never depend on the database framework.");
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
            [
                InfrastructureNamespace,
                ApiNamespace
            ])
            .GetResult();
        
        var failingTypes = result.FailingTypes?.Select(t => t.FullName) ?? [];

        failingTypes.Should().BeNullOrEmpty("Application layer must remain decoupled from Infrastructure and API details.");
    }

    [Fact]
    public void Application_Should_Not_Depend_On_EntityFrameworkCore()
    {
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .DoNotHaveName("IApplicationDbContext")
            .And().DoNotResideInNamespaceContaining("FlowDesk.Application.Features")
            .And().DoNotResideInNamespaceContaining("FlowDesk.Application.EventHandlers")
            .And().DoNotResideInNamespaceContaining("FlowDesk.Application.Behaviours")
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore")
            .GetResult();

        var failingTypes = result.FailingTypes?.Select(t => t.FullName) ?? [];
        failingTypes.Should().BeNullOrEmpty("Core application logic outside features must remain database-agnostic.");
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var assembly = typeof(Infrastructure.AssemblyReference).Assembly;

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApiNamespace)
            .GetResult();
        
        var failingTypes = result.FailingTypes?.Select(t => t.FullName) ?? [];

        failingTypes.Should().BeNullOrEmpty("Infrastructure layer cannot point upwards into the API layer.");
    }

    [Fact]
    public void Controllers_Should_Have_Dependency_On_MediatR()
    {
        var assembly = typeof(Api.AssemblyReference).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Or()
            .Inherit<Microsoft.AspNetCore.Mvc.ControllerBase>()
            .Should()
            .HaveDependencyOnAny("MediatR")
            .GetResult();

        var failingTypes = result.FailingTypes?.Select(t => t.FullName) ?? [];
        failingTypes.Should().BeNullOrEmpty("API Controllers must rely on MediatR to pass requests to the Application layer.");
    }
} 

