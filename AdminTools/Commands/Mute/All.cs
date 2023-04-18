namespace AdminTools.Commands.Mute
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    public class All : ICommand
    {
        public string Command => "all";

        public string[] Aliases { get; } = { "*" };

        public string Description => "Mutes everyone from speaking at all in the server";

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

            foreach (Player player in Player.List)
            {
                if (!player.ReferenceHub.serverRoles.RemoteAdmin)
                    player.IsMuted = true;
            }

            response = "Everyone from the server who is not a staff has been muted completely";
            return true;
        }
    }
}