namespace YsaActivities.Api.Endpoints.Activities;

public class ActivityEndpoints : IEndpointModule
{
    public string ModuleName { get; } = "Activities";

    public void RegisterEndpoints(WebApplication app)
    {
        var group = app.MapGroup("activities");

        group.MapPost("/", CreateActivity)
            .WithName("CreateActivity");
    }

    private static async Task CreateActivity()
    {
    }
}