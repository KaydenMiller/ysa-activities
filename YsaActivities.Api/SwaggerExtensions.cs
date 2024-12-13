using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace YsaActivities.Api;

public static class SwaggerExtensions
{
    public static void AddSchemasForIdentifiersInAssembly<TMarker>(this SwaggerGenOptions options)
    {
        options.AddSchemasForIdentifiers(typeof(TMarker));
    }

    private static void AddSchemasForIdentifiers(this SwaggerGenOptions options, Type type)
    {
        var identifiers = type.Assembly.ExportedTypes
            .Where(t =>
            {
                var attr = t.GetCustomAttributes(typeof(ValueObjectAttribute), false)
                    .SingleOrDefault();
                return attr is not null;
            })
            .Select(a => new
            {
                type = a,
                attr =
                    a.GetCustomAttributes(typeof(ValueObjectAttribute), false).SingleOrDefault() as
                        ValueObjectAttribute
            });

        foreach (var item in identifiers)
        {
            if (item.attr is not null)
            {
                options.MapType(item.type, () => new OpenApiSchema()
                {
                    Type = item.attr?.OpenApiType
                });
            }
        }
    }
}