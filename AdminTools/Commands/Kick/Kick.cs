namespace AdminTools.Commands.Kick
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Kick : ICommand
    {
        public string Command => "kick";

        public string[] Aliases => null;

        public string Description => "Kicks a player from the game with a custom reason";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.kick"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: kick (player id / name) (reason)";
                return false;
            }

            Player ply = Player.Get(arguments.At(0));
            if (ply == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (ply.ReferenceHub.serverRoles.Group != null && ply.ReferenceHub.serverRoles.Group.RequiredKickPower >
                ((CommandSender)sender).KickPower)
            {
                response = "You do not have permission to kick the specified player";
                return false;
            }

            string kickReason = arguments.FormatArguments(1);
            ply.Kick(kickReason);

            response = $"Player {ply.Nickname} has been kicked for \"{kickReason}\"";
            return true;
        }
    }
}