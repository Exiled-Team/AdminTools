using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.Unmute
{
    public class Com : ICommand
    {
        public string Command { get; } = "icom";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Removes intercom mutes everyone in the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.mute"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: punmute icom";
                return false;
            }

            foreach (Player ply in Player.List)
                ply.IsIntercomMuted = false;

            response = "Everyone from the server who is not a staff can speak in the intercom now";
            return true;
        }
    }
}
