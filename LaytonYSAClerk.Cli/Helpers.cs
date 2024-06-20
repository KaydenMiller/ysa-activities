using ConsoleTables;
using LaytonYSAClerk.Cli.Entities;
using LaytonYsaClerk.EmailService;

namespace LaytonYSAClerk.Cli;

public static class Helpers
{
    public static Member ToMember(this ChurchUser user)
    {
        return new Member()
        {
            MemberId = user.MemberId,
            Address = user.Address,
            Birthday = user.Birthday,
            Gender = user.Gender,
            HouseholdPosistion = user.HouseholdPosistion,
            MoveInDate = user.MoveInDate,
            Phone = user.Phone,
            FullName = user.FullName,
            Unit = user.UnitDetails.ToUnit(user.MemberId)
        };
    }

    public static Unit ToUnit(this UnitDetails unitDetails, long memberId)
    {
        return new Unit()
        {
            MemberId = memberId,
            UnitNumber = unitDetails.UnitNumber,
            UnitTitle = unitDetails.UnitTitle,
            LeaderName = unitDetails.LeaderName,
            LeaderEmail = unitDetails.LeaderEmail,
            LeaderCellPhone = unitDetails.LeaderCellPhone,
            PositionName = unitDetails.PositionName
        };
    }

    public static void WriteMembersToTable(this IEnumerable<Member> members)
    {
        var table = new ConsoleTable("Fullname", "Move In Date", "Email Sent On");

        foreach (var member in members)
        {
            table.AddRow(member.FullName,
                member.MoveInDate?.ToString("yyyy MMMM dd") ?? "Unknown",
                member.NewMemberEmailSentDate?.ToString("yyyy MMMM dd") ?? "Not Sent");
        }
        
        table.Write();
    }
}