using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.FriendlyFire
{
    class Disable : ICommand
    {
        public string Command { get; } = "disable";

        public string[] Aliases { get; } = { "off" };

        public string Description { get; } = "Disables Friendly Fire";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.friendlyfire"))
            {
                response = "You do not have permission to use this command. (friendlyfirebetterperms.use)";
                return false;
            }
            else
            {
                ServerConsole.FriendlyFire = false;
                response = "Disabled Friendly Fire";
                Log.Info("FriendlyFire disabled.");
                return true;
            }
        }
    }
}