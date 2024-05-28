using CommandSystem;
using System;

namespace AdminTools.Commands.HintBroadcast
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class HintBroadcast : ParentCommand
    {
        public HintBroadcast() => LoadGeneratedCommands();

        public override string Command { get; } = "hintbroadcast";
        public override string[] Aliases { get; } = new string[] { "hint" , "hbc" };
        public override string Description { get; } = "Broadcasts a message to either a user, a group, a role, all staff, or everyone";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Clear());
            RegisterCommand(new Group());
            RegisterCommand(new User());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response) // TODO: Make it ParentCommand
        {
            response = "Not a valid subcommand. Available subcommands: user, group, clear";
            return false;
        }
    }
}
