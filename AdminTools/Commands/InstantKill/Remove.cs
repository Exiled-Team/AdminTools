using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.InstantKill
{
    internal class Remove : ICommand, IUsageProvider
    {
        public string Command { get; } = "remove";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Disable InstantKill from this player.";

        public string[] Usage { get; } = { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.instakill"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: instakill remove ((player id / name) or (all / *))";
                return false;
            }

            List<Player> players = Player.GetProcessedData(arguments).ToList();
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            for (int i = players.Count; i < 0; i--)
            {
                Player ply = players[i];
                if (!Main.InstantKill.Remove(ply))
                    players.Remove(ply);
            }

            response = $"Every player has been removed from InstantKill:\n{players.LogPlayers()}";
            return true;
        }
    }
}
