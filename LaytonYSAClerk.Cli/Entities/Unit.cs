using System.ComponentModel.DataAnnotations.Schema;

namespace LaytonYSAClerk.Cli.Entities;

public class Unit
{
    [Column("unitNumber")]
    public int UnitNumber { get; set; }
    
    [Column("title")]
    public string? UnitTitle { get; set; } = default!;
    
    [Column("leaderName")]
    public string LeaderName { get; set; } = default!;
    
    [Column("leaderCellPhone")]
    public string? LeaderCellPhone { get; set; } = default!;

    [Column("leaderEmail")]
    public string LeaderEmail { get; set; } = default!;

    [Column("positionName")]
    public string PositionName { get; set; } = default!;

    [Column("memberId")]
    public long MemberId { get; set; }
    public virtual Member Member { get; set; } = default!;
}