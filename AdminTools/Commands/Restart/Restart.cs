using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.Restart
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Restart : ParentCommand
    {
        public Restart() => LoadGeneratedCommands();

        public override string Command { get; } = "restart";

        public override string[] Aliases { get; } = new string[] { "rt" };

        public override string Description { get; } = "Restart the server automatically at the end of the round";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.restart"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: restart";
                return false;
            }

            if (Plugin.RestartOnEnd)
            {
                Plugin.RestartOnEnd = false;
                response = "Automatic restart deactivated";
                return true;
            }

            Plugin.RestartOnEnd = true;
            response = "Automatic restart activated";
            return true;
        }
    }
}