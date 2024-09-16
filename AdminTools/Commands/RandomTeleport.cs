using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class RandomTeleport : ICommand, IUsageProvider
    {
        public string Command { get; } = "randomtp";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Randomly teleports a user or all users to a random room in the facility";

        public string[] Usage { get; } = { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "randomtp", PlayerPermissions.PlayersManagement, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: randomtp ((player id / name) or (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            foreach (Player ply in players)
            {
                ply.RandomTeleport(typeof(Room));
            }

            response = $"All specified players have been teleported to a random room in the facility:\n{players.LogPlayers()}";
            return true;
        }
    }
}
