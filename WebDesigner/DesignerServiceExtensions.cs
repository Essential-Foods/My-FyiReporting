using Microsoft.Extensions.DependencyInjection;

namespace Majorsilence.Reporting.WebDesigner;

public static class DesignerServiceExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="RdlDesignerOptions"/> so it is available to the
    /// endpoint handlers via DI.  Call before <see cref="DesignerEndpoints.MapRdlDesigner"/>.
    /// </summary>
    public static IServiceCollection AddRdlDesigner(
        this IServiceCollection services,
        Action<RdlDesignerOptions>? configure = null)
    {
        var opts = new RdlDesignerOptions();
        configure?.Invoke(opts);
        services.AddSingleton(opts);
        return services;
    }
}
