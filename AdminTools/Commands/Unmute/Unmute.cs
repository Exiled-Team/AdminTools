namespace AdminTools.Commands.Unmute
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Unmute : ParentCommand
    {
        public Unmute()
        {
            LoadGeneratedCommands();
        }

        public override string Command => "punmute";

        public override string[] Aliases => null;

        public override string Description => "Unmutes everyone from speaking or by intercom in the server";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new All());
            RegisterCommand(new Com());
            RegisterCommand(new RoundStart());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender,
            out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.mute"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            response = "Invalid subcommand. Available ones: icom, all, roundstart";
            return false;
        }
    }
}