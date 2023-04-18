namespace AdminTools.Commands.Ahp
{
    using System;
    using System.Collections.Generic;
    using CommandSystem;
    using Exiled.API.Features;
    using RemoteAdmin;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ahp : ICommand
    {
        public string Command => "ahp";

        public string[] Aliases => null;

        public string Description => "Sets a user or users Artificial HP to a specified value";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions((CommandSender)sender, "ahp",
                    PlayerPermissions.PlayersManagement, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 2)
            {
                response = "Usage: ahp ((player id / name) or (all / *)) (value)";
                return false;
            }

            List<Player> players = new();
            if (!float.TryParse(arguments.At(1), out float value))
            {
                response = $"Invalid value for AHP: {value}";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    foreach (Player ply in Player.List)
                    {
                        players.Add(ply);
                    }

                    break;
                default:
                    Player player = Player.Get(arguments.At(0));
                    if (player == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    players.Add(player);
                    break;
            }

            response = string.Empty;
            foreach (Player p in players)
            {
                p.ArtificialHealth = value;
                response += $"\n{p.Nickname}'s AHP has been set to {value}";
            }

            return true;
        }
    }
}