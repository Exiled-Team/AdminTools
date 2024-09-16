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
    public class TargetGhost : ICommand
    {
        public const string HelpStr = "Usage: targetghost (player id / name) (player id / name) ...";

        public string Command { get; } = "targetghost";

        public string[] Aliases { get; } = { "tg" };

        public string Description { get; } = "Sets a user to be invisible to another user";

        public string[] Usage { get; } = { "%player%", "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.targetghost"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = HelpStr;
                return false;
            }

            IEnumerable<Player> sourcePlayers = Player.GetProcessedData(arguments);

            if (sourcePlayers.IsEmpty())
            {
                response = $"Invalid source player: {arguments.At(0)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments, 1);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            foreach (Player sourcePlayer in sourcePlayers)
            {
                foreach (Player victim in players)
                {
                    if (sourcePlayer.Role.Is(out FpcRole role))
                    {
                        if (!role.IsInvisibleFor.Add(victim))
                            role.IsInvisibleFor.Remove(victim);
                    }
                }
            }

            response = "Finshed Ghostbusting.";
            return true;
        }
    }
}