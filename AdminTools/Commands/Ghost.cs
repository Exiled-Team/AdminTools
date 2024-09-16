using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ghost : ICommand, IUsageProvider
    {
        public string Command { get; } = "ghost";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Sets everyone or a user to be invisible";

        public string[] Usage { get; } = { "%player% / Clear", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.ghost"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage:\nghost ((player id / name) or (all / *))" +
                    "\nghost clear";
                return false;
            }

            switch (arguments.At(0))
            {
                case "clear":
                    {
                        foreach (Player pl in Player.List)
                            if (pl.Role is FpcRole fpc)
                                fpc.IsInvisible = false;

                        response = "Everyone is no longer invisible";
                        return true;
                    }
                default:
                    {
                        IEnumerable<Player> players = Player.GetProcessedData(arguments);
                        if (players.IsEmpty())
                        {
                            response = $"Player not found: {arguments.At(0)}";
                            return false;
                        }
                        foreach (Player pl in players)
                            if (pl.Role is FpcRole fpc)
                                fpc.IsInvisible = !fpc.IsInvisible;
                        
                        response = $"The following player has been ghosted:\n{players.LogPlayers()}";
                        return true;
                    }
            }
        }
    }
}

