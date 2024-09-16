using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using UnityEngine;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Scale : ICommand, IUsageProvider
    {
        public string Command { get; } = "scale";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Scales a specific player or all players.";

        public string[] Usage { get; } = { "%player%", "size" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.size"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage:\nscale ((player id / name) or (all / *)) (value)" +
                    "\nscale reset";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float scale))
            {
                response = $"Invalid value for scale: {arguments.At(1)}";
                return false;
            }

            Vector3 size = Vector3.one * scale;
            foreach (Player ply in players)
                ply.Scale = size;

            response = $"The specified player's size has been set to {size}:\n{players.LogPlayers()}";
            return true;
        }
    }
}
