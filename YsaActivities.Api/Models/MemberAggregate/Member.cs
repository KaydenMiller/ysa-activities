using MongoDB.Bson;
using YsaActivities.Api.Exceptions;
using YsaActivities.Api.Models.MemberAggregate;

namespace YsaActivities.Api.Models;

public record struct MemberId(ObjectId Id);

public class Member
{
    /// <summary>
    /// This is our global id
    /// </summary>
    public MemberId Id { get; set; }
    
    /// <summary>
    /// The full name of the member.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The known physical address of the member.
    /// </summary>
    public string Address { get; set; }
    
    public Gender Gender { get; set; }

    /// <summary>
    /// The notes that have been written about this member
    /// </summary>
    public List<Note> Notes { get; } = [];

    public void WriteNote(string note, MemberId author)
    {
        Notes.Add(new Note()
        {
            Id = new NoteId(ObjectId.GenerateNewId()),
            Author = author,
            Content = note,
            Created = DateTime.Now,
        });
    }

    public void UpdateNote(NoteId noteId, MemberId author, string content)
    {
        var note = Notes.SingleOrDefault(n => n.Id == noteId && n.Author == author);
        if (note is null)
            throw new NotFoundException($"Note with noteId {noteId} or authorId {author} was not found");
        note.Content = content;
    }
}