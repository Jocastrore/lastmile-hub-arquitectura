using LastMileHub.Application.Dispatch;
using LastMileHub.Domain.Dispatch;

namespace LastMileHub.Tests;

/// <summary>Fitness functions: reglas de arquitectura verificadas automáticamente.</summary>
public class ArchitectureTests
{
    private static IEnumerable<string> References(System.Reflection.Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(a => a.Name ?? string.Empty);

    [Fact]
    public void Domain_depends_on_nothing_outside_the_runtime()
    {
        var refs = References(typeof(Delivery).Assembly).ToList();

        Assert.DoesNotContain(refs, n => n.StartsWith("LastMileHub."));
        Assert.DoesNotContain(refs, n => n.StartsWith("Microsoft."));
    }

    [Fact]
    public void Application_depends_only_on_Domain()
    {
        var refs = References(typeof(AssignCourierUseCase).Assembly).ToList();

        Assert.Contains("LastMileHub.Domain", refs);
        Assert.DoesNotContain(refs, n => n == "LastMileHub.Infrastructure" || n == "LastMileHub.Api");
        Assert.DoesNotContain(refs, n => n.StartsWith("Microsoft.Data"));
        Assert.DoesNotContain(refs, n => n.StartsWith("Microsoft.AspNetCore"));
    }

    [Fact]
    public void UseCase_receives_only_abstractions_by_constructor()
    {
        var parameters = typeof(AssignCourierUseCase).GetConstructors().Single().GetParameters();

        Assert.All(parameters, p =>
            Assert.True(p.ParameterType.IsInterface || p.ParameterType == typeof(TimeProvider),
                $"{p.ParameterType.Name} should be an interface"));
    }
}
