using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using Exiled.API.Features.Pickups;
using System.Collections.Generic;
using System.Linq;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class DropItem : ICommand, IUsageProvider
    {
        public string Command { get; } = "dropitem";

        public string[] Aliases { get; } = new string[] { "drop", "dropi" };

        public string Description { get; } = "Drops a specified amount of a specified item on either all users or a specific user";

        public string[] Usage { get; } = new string[] { "%player%", "%item%", "[amount = 1]" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
                return false;

            if (arguments.Count < 2)
            {
                response = "Usage: dropitem (all / *) (ItemType) [amount = 1]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }
            if (!Enum.TryParse(arguments.At(1), true, out ItemType item))
            {
                response = $"Invalid value for item type: {arguments.At(1)}";
                return false;
            }

            if (!uint.TryParse(arguments.ElementAtOrDefault(2), out uint amount))
                amount = 1;

            foreach (Player ply in players)
                for (int i = 0; i < amount; i++)
                    Pickup.CreateAndSpawn(item, ply.Position, ply.Rotation, ply);

            response = $"{amount} of {item} was spawned on all the following player:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
