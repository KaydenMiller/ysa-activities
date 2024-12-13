using System.Globalization;
using System.Text.RegularExpressions;

namespace YsaActivities.Api.RouteConstraints;

public partial class NoteRouteConstraint : IRouteConstraint 
{
    private readonly ILogger<NoteRouteConstraint> _logger;

    public NoteRouteConstraint(ILogger<NoteRouteConstraint> logger)
    {
        _logger = logger;
    }
    
    [GeneratedRegex(@"^(.*)$")]
    private static partial Regex GetRegex();

    public bool Match(HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        ArgumentNullException.ThrowIfNull(routeKey);
        ArgumentNullException.ThrowIfNull(values);

        if (!values.TryGetValue(routeKey, out var routeValue))
            return false;
        _logger.LogTrace("Attempting to parse {NoteId} as a NoteId", routeValue);

        var parameterValueString = Convert.ToString(routeValue, CultureInfo.InvariantCulture);
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterValueString);
            
        var match = GetRegex().Match(parameterValueString);
        var userGuidString = match.Groups[1].Value;
        var parsedSuccessfully = Guid.TryParse(userGuidString, out _);

        return parsedSuccessfully;
    }
}