using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ahp : ICommand, IUsageProvider
    {
        public string Command { get; } = "ahp";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Sets a user or users Artificial HP to a specified value";

        public string[] Usage { get; } = new string[] { "%player%", "Value", "[decay = 1.2]", "[efficacy = 0.7]", "[sustain = 0]", "[IsPersistant = false]" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
                return false;

            if (arguments.Count < 2)
            {
                response = "Usage: ahp ((player id / name) or (all / *)) (value) [decay = 1.2] [efficacy = 0.7] [sustain = 0] [persistant = false]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float value))
            {
                response = $"Invalid value for AHP: {value}";
                return false;
            }

            if (!float.TryParse(arguments.ElementAtOrDefault(3), out float decay))
                decay = 1.2f;

            if (!float.TryParse(arguments.ElementAtOrDefault(4), out float efficacy))
                efficacy = 0.7f;

            float.TryParse(arguments.ElementAtOrDefault(5), out float sustain);

            bool.TryParse(arguments.ElementAtOrDefault(6), out bool persistant);

            foreach (Player p in players)
            {
                p.AddAhp(value, value, decay, efficacy, sustain, persistant);
            }
            response = $"AHP has been set to {value} for all the followed players:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
