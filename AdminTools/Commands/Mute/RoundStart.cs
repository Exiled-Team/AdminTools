namespace AdminTools.Commands.Mute
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    public class RoundStart : ICommand
    {
        public string Command => "roundstart";

        public string[] Aliases { get; } = { "rs" };

        public string Description => "Mutes everyone from speaking until the round starts.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.mute"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: pmute roundstart";
                return false;
            }

            if (Round.IsStarted)
            {
                response = "You cannot use this command after the round has started!";
                return false;
            }

            foreach (Player player in Player.List)
            {
                if (!player.IsMuted && !player.ReferenceHub.serverRoles.RemoteAdmin)
                {
                    player.IsMuted = true;
                    Plugin.RoundStartMutes.Add(player);
                }
            }

            response = "All non-staff players have been muted until the round starts.";
            return true;
        }
    }
}