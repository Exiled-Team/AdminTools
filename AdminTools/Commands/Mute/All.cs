using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Mute
{
    public class All : ICommand
    {
        public string Command { get; } = "all";

        public string[] Aliases { get; } = new string[] { "*" };

        public string Description { get; } = "Mutes everyone from speaking at all in the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.mute"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: pmute all";
                return false;
            }

            foreach (var player in Player.List)
                if (!player.ReferenceHub.serverRoles.RemoteAdmin)
                    player.IsMuted = true;

            response = "Everyone from the server who is not a staff has been muted completely";
            return true;
        }
    }
}
