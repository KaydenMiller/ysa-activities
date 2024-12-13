namespace YsaActivities.Api.Endpoints;

public static class EndpointExtensions
{
    public static void AddEndpointModulesAssemblyWithType<TMarker>(this IServiceCollection services)
    {
        services.AddEndpointModules(typeof(TMarker));
    }

    public static void AddEndpointModules(this IServiceCollection services, Type type)
    {
        var modules = type.Assembly.ExportedTypes
            .Where(t => typeof(IEndpointModule).IsAssignableFrom(t) &&
                        t.IsClass &&
                        !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IEndpointModule>()
            .ToList();

        services.AddSingleton(modules as IReadOnlyCollection<IEndpointModule>);
    }

    public static void UseEndpointModules(this WebApplication app)
    {
        var modules = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointModule>>();

        foreach (var endpointModule in modules)
        {
            Console.WriteLine(endpointModule.ModuleName);
            endpointModule.RegisterEndpoints(app);
        }
    }
}