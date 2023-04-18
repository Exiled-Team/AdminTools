namespace AdminTools.Commands.BreakDoors
{
    using System;
    using System.Collections.Generic;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class BreakDoors : ICommand
    {
        public const string BreakDoorsSessionVariableName = "AT-BreakDoors";

        public string Command => "breakdoors";

        public string[] Aliases { get; } = { "bd" };

        public string Description => "Manage breaking door/gate properties for players";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
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
                    foreach (Player player in Player.List)
                    {
                        players.Add(player);
                    }

                    break;
                default:
                    Player ply = Player.Get(arguments.At(0));

                    if (ply is null)
                    {
                        response = $"Player {arguments.At(0)} not found.";
                        return false;
                    }

                    players.Add(ply);

                    break;
            }

            foreach (Player player in players)
            {
                if (player.HasSessionVariable(BreakDoorsSessionVariableName))
                {
                    player.SessionVariables.Remove(BreakDoorsSessionVariableName);
                    continue;
                }

                player.AddBooleanSessionVariable(BreakDoorsSessionVariableName);
            }

            response =
                $"{players.Count} players have been updated. (Players with BD were removed, those without it were added)";
            return true;
        }
    }
}