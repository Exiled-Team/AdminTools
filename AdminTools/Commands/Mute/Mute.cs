namespace AdminTools.Commands.Mute
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Mute : ParentCommand
    {
        public Mute()
        {
            LoadGeneratedCommands();
        }

        public override string Command => "pmute";

        public override string[] Aliases => null;

        public override string Description => "Mutes everyone from speaking or by intercom in the server";

        public sealed override void LoadGeneratedCommands()
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