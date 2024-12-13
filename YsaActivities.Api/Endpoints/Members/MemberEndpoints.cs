using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using YsaActivities.Api.Models;
using YsaActivities.Api.Models.MemberAggregate;
using YsaActivities.Api.Services.Repositories;

namespace YsaActivities.Api.Endpoints.Members;

public class MemberEndpoints : IEndpointModule
{
    public string ModuleName { get; } = "Members";
    
    public void RegisterEndpoints(WebApplication app)
    {
        var group = app.MapGroup("members");

        group.MapPost("/{memberId}/note", AddNoteToMember)
            .WithName("AddNoteToMember");
        
        group.MapPut("/{memberId}/note/{noteId:noteid}", UpdateNoteOnMember) 
            .WithName("UpdateNoteOnMember");
    }

    private static async Task<NoContent> AddNoteToMember(
        [FromServices] MemberRepository repository,
        [FromRoute] string memberId,
        [FromBody] WriteNoteRequest request)
    {
        var member = await repository.GetMemberById(new MemberId(ObjectId.Parse(memberId)));
        member.WriteNote(request.Content, request.Author);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> UpdateNoteOnMember(
        [FromServices] MemberRepository repository,
        [FromRoute] MemberId memberId,
        [FromRoute] NoteId noteId,
        [FromBody] UpdateNoteRequest request)
    {
        var member = await repository.GetMemberById(memberId);
        member.UpdateNote(noteId, request.Author, request.Content);
        return TypedResults.NoContent();
    }
}

public class WriteNoteRequest
{
    public MemberId Author { get; set; }
    public string Content { get; set; }
}

public class UpdateNoteRequest : WriteNoteRequest
{
    public NoteId NoteId { get; set; }
}