using Cocona;
using LaytonYSAClerk.Cli.Commands.Members;

namespace LaytonYSAClerk.Cli.Commands;

public static class RegisterCommands
{
    public static void RegisterMembersCommand(this CoconaApp app)
    {
        app.AddSubCommand("member", memberCommand =>
        {
            memberCommand.AddCommand("update", MembersCommandHandler.UpdateMembers);
            memberCommand.AddCommand("list", MembersCommandHandler.ListMembers);
            memberCommand.AddCommand("email", MembersCommandHandler.EmailMembersBishops);

            memberCommand.AddSubCommand("set", (setCommand) =>
            {
                setCommand.AddCommand("status", MembersCommandHandler.SetStatus);
            });
        });
    }
}