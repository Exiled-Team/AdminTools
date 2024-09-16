using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;

namespace AdminTools.Commands.Inventory
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Drop : ICommand, IUsageProvider
    {
        public string Command { get; } = "drop";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Drops the items in a players inventory";

        public string[] Usage { get; } = { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
                return false;

            if (arguments.Count != 1)
            {
                response = "Usage: inventory drop ((player id / name) or (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            foreach (Player p in players)
                p.DropItems();

            response = $"All items have been dropped from the following players: \n{players.LogPlayers()}";
            return true;
        }
    }
}