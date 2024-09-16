using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class BreakDoors : ICommand, IUsageProvider
    {
        public string Command { get; } = "breakdoors";

        public string[] Aliases { get; } = { "bd" };

        public string Description { get; } = "Manage breaking door/gate properties for players";

        public string[] Usage { get; } = { "%player%", "IsEnable" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.bd"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "Usage: breakdoors (all / *) [IsEnable]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.ElementAtOrDefault(0)}";
                return false;
            }

            bool? isJail = null;
            if (bool.TryParse(arguments.ElementAtOrDefault(1), out bool result))
                isJail = result;

            foreach (Player player in players)
                if (isJail is true)
                {
                    if (Main.BreakDoors.Contains(player))
                        Main.BreakDoors.Add(player);
                }
                else if (isJail is false)
                {
                    Main.BreakDoors.Remove(player);
                }
                else
                {
                    if (!Main.BreakDoors.Remove(player))
                        Main.BreakDoors.Add(player);
                }

            response = $"BreakDoor has been enable for all the followed player:\n{players.LogPlayers()}";
            return true;
        }
    }
}
