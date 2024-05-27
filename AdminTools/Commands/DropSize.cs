using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using Exiled.API.Features.Pickups;
using System.Collections.Generic;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class DropSize : ICommand, IUsageProvider
    {
        public string Command { get; } = "dropsize";

        public string[] Aliases { get; } = new string[] { "drops" };

        public string Description { get; } = "Drops a selected amount of a selected item on a specific user or all users";

        public string[] Usage { get; } = new string[] { "%player%", "%item%", "size", "[size]", "[size]" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
                return false;

            if (arguments.Count < 3)
            {
                response = "Usage:\ndropsize ((player id / name) or (all / *)) (ItemType) (size)\ndropsize ((player id / name) or (all / *)) (ItemType) (x size) (y size) (z size)";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out ItemType type))
            {
                response = $"Invalid value for item name: {arguments.At(1)}";
                return false;
            }

            switch (arguments.Count)
            {
                case 3:
                    if (!float.TryParse(arguments.At(2), out float size))
                    {
                        response = $"Invalid value for item scale: {arguments.At(2)}";
                        return false;
                    }
                    SpawnItem(players, type, size, out response);
                    return true;
                case 5:
                    if (!float.TryParse(arguments.At(2), out float xval))
                    {
                        response = $"Invalid value for item scale: {arguments.At(2)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(3), out float yval))
                    {
                        response = $"Invalid value for item scale: {arguments.At(3)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(4), out float zval))
                    {
                        response = $"Invalid value for item scale: {arguments.At(4)}";
                        return false;
                    }
                    SpawnItem(players, type, xval, yval, zval, out response);
                    return true;
                default:
                    response = "\nUsage:\ndrops (all / *) (ItemType) (size) \ndrops (all / *) (ItemType) (x size) (y size) (z size)";
                    return false;
            }
        }

        private void SpawnItem(IEnumerable<Player> players, ItemType type, float size, out string message)
            => SpawnItem(players, type, size, size, size, out message);

        private void SpawnItem(IEnumerable<Player> players, ItemType type, float x, float y, float z, out string message)
        {
            foreach (Player ply in players)
            {
                if (ply.IsDead)
                    continue;

                Pickup.CreateAndSpawn(type, ply.Position, default, ply).Scale = new(x, y, z);
            }
            message = $"Spawned a {type} that is ({x}, {y}, {z}) at all the following player:\n{players.LogPlayers()}";
        }
    }
}
