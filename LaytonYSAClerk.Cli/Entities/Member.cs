using System.ComponentModel.DataAnnotations.Schema;

namespace LaytonYSAClerk.Cli.Entities;

public class Member
{
    [Column("id")]
    public long MemberId { get; set; }

    [Column("newMemberEmailSentDate")]
    public DateTime? NewMemberEmailSentDate { get; set; } = null;
    
    [Column("textAddress")]
    public string? Address { get; set; } = default!;

    [Column("birthdate")]
    public DateOnly? Birthday { get; set; }

    [Column("gender")]
    public string? Gender { get; set; } = default!;

    [Column("householdPosition")]
    public string? HouseholdPosistion { get; set; } = default!;

    [Column("moveDateCalc")]
    public DateOnly? MoveInDate { get; set; }

    [Column("phone")]
    public string? Phone { get; set; } = default!;

    [Column("name")]
    public string FullName { get; set; } = default!;

    [Column("priorUnitName")]
    public string? PriorUnit { get; set; } = default!;

    [Column("priorUnitNumber")]
    public string? PriorUnitNumber { get; set; } = default!;
    
    public virtual Unit? Unit { get; set; } = default!;
}