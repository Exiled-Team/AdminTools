using System;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.Unmute
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Unmute : ParentCommand, IUsageProvider
    {
        public Unmute() => LoadGeneratedCommands();

        public override string Command { get; } = "punmute";

        public override string[] Aliases { get; } = Array.Empty<string>();

        public override string Description { get; } = "Unmutes everyone from speaking or by intercom in the server";

        public string[] Usage { get; } = { "all/icom/roundstart" };

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
