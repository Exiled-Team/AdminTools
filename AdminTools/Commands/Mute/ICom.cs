namespace AdminTools.Commands.Mute
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    public class Com : ICommand
    {
        public string Command => "icom";

        public string[] Aliases => null;

        public string Description => "Intercom mutes everyone in the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.mute"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: pmute icom";
                return false;
            }

            foreach (Player player in Player.List)
            {
                if (!player.ReferenceHub.serverRoles.RemoteAdmin)
                    player.IsIntercomMuted = true;
            }

            response = "Everyone from the server who is not a staff has been intercom muted";
            return true;
        }
    }
}