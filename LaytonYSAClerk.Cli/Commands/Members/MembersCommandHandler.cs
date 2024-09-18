using Cocona;
using LaytonYSAClerk.Cli.Entities;
using LaytonYSAClerk.Cli.Services;

namespace LaytonYSAClerk.Cli.Commands.Members;

public class MembersCommandHandler
{
    public static async Task UpdateMembers(
        [FromService] MembersRepository membersRepository,
        [FromService] WebsiteService websiteService)
    {
        var members = await websiteService.GetMembersFromWebsite();
        foreach (var member in members)
        {
            await membersRepository.AddMember(member.ToMember());
        }
    }

    public static async Task ListMembers(
        [FromService] MembersRepository membersRepository)
    {
        var members = await membersRepository.GetMembers();
        members
           .OrderBy(m => m.MoveInDate)
           .WriteMembersToTable();
    }

    public static async Task EmailMembersBishops(
        [FromService] MembersRepository membersRepository,
        [FromService] EmailService emailService)
    {
        var members = await membersRepository.GetMembersToEmail();
        members.WriteMembersToTable();

        List<Member> failedMembers = [];
        foreach (var member in members)
        {
            var result = await emailService.SendEmailForMember(member);
            if (!result.IsError)
            {
                member.NewMemberEmailSentDate = DateTime.UtcNow;
                await membersRepository.UpdateMember(member);
            }
            else
            {
                failedMembers.Add(member);
            }
        }
        
        failedMembers.WriteMembersToTable();
    }

    public static async Task SetStatus(
        [Option("all")] bool allMembers,
        [Option("member")] long? memberId,
        [FromService] MembersRepository membersRepository)
    {
        if (memberId is not null)
        {
            var member = await membersRepository.GetMember(memberId.Value);
            if (member is null)
            {
                throw new Exception("Member with id not found");
            }
            
            member.NewMemberEmailSentDate = DateTime.UtcNow;
            await membersRepository.UpdateMember(member);
        }

        if (allMembers)
        {
            var members = await membersRepository.GetMembers();
            var needToEmailMembers = members.Where(m => m.NewMemberEmailSentDate is null);
            foreach (var member in needToEmailMembers)
            {
                member.NewMemberEmailSentDate = DateTime.UtcNow;
                await membersRepository.UpdateMember(member);
            }
        }
        else
        {
            throw new Exception("Missing required option");
        }
    }
}