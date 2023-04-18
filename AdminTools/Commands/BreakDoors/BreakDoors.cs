using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.BreakDoors
{
    using System.Collections.Generic;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class BreakDoors : ParentCommand
    {
        public BreakDoors() => LoadGeneratedCommands();

        public override string Command { get; } = "breakdoors";

        public override string[] Aliases { get; } = new string[] { "bd" };

        public override string Description { get; } = "Manage breaking door/gate properties for players";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.bd"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            List<Player> players = new();
            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    foreach (var player in Player.List) 
                        players.Add(player);

                    break;
                default:
                    var ply = Player.Get(arguments.At(0));
                    if (ply is null)
                    {
                        response = $"Player {arguments.At(0)} not found.";
                        return false;
                    }

                    players.Add(ply);

                    break;
            }

            foreach (var player in players)
                if (EventHandlers.BreakDoorsList.Contains(player))
                    EventHandlers.BreakDoorsList.Remove(player);
                else
                    EventHandlers.BreakDoorsList.Add(player);

            response =
                $"{players.Count} players have been updated. (Players with BD were removed, those without it were added)";
            return true;
        }
    }
}
