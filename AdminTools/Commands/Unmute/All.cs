using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.Unmute
{
    public class All : ICommand
    {
        public string Command { get; } = "all";

        public string[] Aliases { get; } = { "*" };

        public string Description { get; } = "Removes all mutes from everyone in the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.mute"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: punmute all";
                return false;
            }

            foreach (Player ply in Player.List)
                if (!ply.ReferenceHub.serverRoles.RemoteAdmin)
                {
                    ply.IsIntercomMuted = false;
                    ply.IsMuted = false;
                }

            response = "Everyone from the server who is not a staff can now speak freely";
            return true;
        }
    }
}
