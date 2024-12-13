using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using YsaActivities.Api.Models;
using YsaActivities.Api.Services.Repositories;

namespace YsaActivities.Api.Endpoints.Units;

public class UnitEndpoints : IEndpointModule
{
    public string ModuleName { get; } = "Units";
    
    public void RegisterEndpoints(WebApplication app)
    {
        var group = app.MapGroup("units");

        group.MapPost("/", CreateUnit)
            .WithName("CreateUnit");
        
        group.MapGet("/", GetAllUnits)
            .WithName("GetAllUnits");
        
        group.MapGet("/{UnitId}", GetUnitById)
            .WithName("GetUnitById");
        
        group.MapPost("/{UnitId}/member", AddMemberToUnit)
            .WithName("AddMemberToUnit");
        
        group.MapDelete("/{UnitId}/member/{MemberId}", RemoveMemberFromUnit)
            .WithName("RemoveMemberFromUnit");
    }

    private async Task<Ok<IEnumerable<Unit>>> GetAllUnits(
        [FromServices] UnitRepository repository)
    {
        var units = await repository.GetAllUnits();
        return TypedResults.Ok(units);
    }

    private async Task<Ok<Unit>> GetUnitById(
        [FromServices] UnitRepository repository,
        [FromRoute] string unitId)
    {
        var id = new UnitId(ObjectId.Parse(unitId));
        var unit = await repository.FindById(id);
        return TypedResults.Ok(unit);
    }

    private async Task<CreatedAtRoute> CreateUnit(
        [FromServices] UnitRepository repository, 
        [FromBody] CreateUnitRequest createUnitRequest)
    {
        var unit = Unit.Create(createUnitRequest.Name);
        await repository.CreateUnit(unit);
        return TypedResults.CreatedAtRoute("GetUnitById", new { unitId = unit.Id.ToString() });
    }

    private async Task<NoContent> AddMemberToUnit(
        [FromServices] UnitRepository repository,
        [FromRoute] string unitId,
        [FromBody] Member member)
    {
        var id = new UnitId(ObjectId.Parse(unitId));
        await repository.AddMemberToUnit(id, member);
        return TypedResults.NoContent(); 
    }

    private async Task<NoContent> RemoveMemberFromUnit(
        [FromServices] UnitRepository repository,
        [FromRoute] string unitId,
        [FromRoute] string memberId)
    {
        await repository.RemoveMemberFromUnit(
            new UnitId(ObjectId.Parse(unitId)),
            new MemberId(ObjectId.Parse(memberId)));
        return TypedResults.NoContent();
    }
}