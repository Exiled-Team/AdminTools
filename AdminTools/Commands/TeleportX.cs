using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class TeleportX : ICommand, IUsageProvider
    {
        public string Command { get; } = "teleportx";

        public string[] Aliases { get; } = { "tpx", "tpto" };

        public string Description { get; } = "Teleports all users or a user to another user";

        public string[] Usage { get; } = { "%player%", "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
                return false;

            if (arguments.Count != 2)
            {
                response = "Usage: teleportx (People teleported: (player id / name) or (all / *)) (Teleported to: (player id / name))";
                return false;
            }

            Player ply = Player.GetProcessedData(arguments, 1).GetRandomValue();
            if (ply == null)
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            foreach (Player plyr in players)
            {
                plyr.Position = ply.Position;
            }

            response = $"The specified players has been teleported to {ply.Nickname}({ply.Id}):\n{players.LogPlayers()}";
            return true;
        }
    }
}
