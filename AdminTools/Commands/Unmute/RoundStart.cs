using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Unmute
{
    public class RoundStart : ICommand
    {
        public string Command { get; } = "roundstart";

        public string[] Aliases { get; } = new string[] { "rs" };

        public string Description { get; } = "Unmutes everyone from speaking until the round starts.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.mute"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: pumute roundstart";
                return false;
            }

            foreach (Player player in Main.RoundStartMutes)
            {
                player.IsMuted = false;
            }
            Main.RoundStartMutes.Clear();

            response = "All non-staff players that were muted until round start have been unmuted.";
            return true;
        }
    }
}
