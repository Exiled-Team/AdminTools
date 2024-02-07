﻿using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;

namespace AdminTools.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ahp : ICommand
    {
        public string Command { get; } = "ahp";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Sets a user or users Artificial HP to a specified value";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
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

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            if (!float.TryParse(arguments.At(1), out float value))
            {
                response = $"Invalid value for AHP: {value}";
                return false;
            }

            if (!float.TryParse(arguments.At(2), out float limit))
                limit = 75f;


            if (!float.TryParse(arguments.At(3), out float decay))
                decay = 1.2f;


            if (!float.TryParse(arguments.At(4), out float efficacy))
                efficacy = 0.7f;

            float.TryParse(arguments.At(5), out float sustain);

            bool.TryParse(arguments.At(6), out bool persistant);

            response = string.Empty;
            foreach (Player p in players)
            {
                p.AddAhp(value, limit, decay, efficacy, sustain, persistant);
                response += $"\n{p.Nickname}'s AHP has been set to {value}";
            }

            return true;
        }
    }
}