using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Explode : ICommand, IUsageProvider
    {
        public string Command { get; } = "explode";

        public string[] Aliases { get; } = { "expl", "boom" };

        public string Description { get; } = "Explodes a specified user or everyone instantly";

        public string[] Usage { get; } = { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.explode"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: expl ((player id / name) or (all / *))";
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
                if (ply.IsDead)
                    continue;

                ply.Explode();
                ply.Kill("Exploded by admin.");
            }
            response = $"The following players have been exploded:\n{players.LogPlayers()}";
            return true;
        }
    }
}
