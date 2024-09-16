using System;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.Mute
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Mute : ParentCommand
    {
        public Mute() => LoadGeneratedCommands();

        public override string Command { get; } = "pmute";

        public override string[] Aliases { get; } = Array.Empty<string>();

        public override string Description { get; } = "Mutes everyone from speaking or by intercom in the server";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new All());
            RegisterCommand(new Com());
            RegisterCommand(new RoundStart());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.mute"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            response = "Invalid subcommand. Available ones: icom, all, roundstart";
            return false;
        }
    }
}
