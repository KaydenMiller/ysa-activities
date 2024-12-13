namespace YsaActivities.Api.Endpoints;

public interface IEndpointModule
{
    public string ModuleName { get; }
    public void RegisterEndpoints(WebApplication app);
}