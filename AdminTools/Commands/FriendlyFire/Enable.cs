
using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.FriendlyFire
{
    class Enable : ICommand
    {
        public string Command { get; } = "enable";

        public string[] Aliases { get; } = { "on" };

        public string Description { get; } = "Enables Friendly Fire";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.friendlyfire"))
            {
                response = "You do not have permission to use this command. (friendlyfirebetterperms.use)";
                return false;
            }
            else
            {
                ServerConsole.FriendlyFire = true;
                response = "Enabled Friendly Fire";
                Log.Info("FriendlyFire enabled.");
                return true;
            }
        }
    }
}