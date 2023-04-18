using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;

namespace AdminTools.Commands.Ahp
{
    using System.Collections.Generic;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ahp : ParentCommand
    {
        public Ahp() => LoadGeneratedCommands();

        public override string Command { get; } = "ahp";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Sets a user or users Artificial HP to a specified value";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "ahp", PlayerPermissions.PlayersManagement, "AdminTools", false))
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
            if (!float.TryParse(arguments.At(1), out var value))
            {
                response = $"Invalid value for AHP: {value}";
                return false;
            }
            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    foreach (var ply in Player.List)
                        players.Add(ply);
                    break;
                default:
                    var player = Player.Get(arguments.At(0));
                    if (player == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    players.Add(player);
                    break;
            }

            response = string.Empty;
            foreach (var p in players)
            {
                p.ArtificialHealth = value;
                response += $"\n{p.Nickname}'s AHP has been set to {value}";
            }

            return true;
        }
    }
}
