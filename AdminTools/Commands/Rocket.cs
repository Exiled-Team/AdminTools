using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using UnityEngine;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Rocket : ICommand, IUsageProvider
    {
        public string Command { get; } = "rocket";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Turns the player into a firework.";

        public string[] Usage { get; } = { "%player%", "speed" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.rocket"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: rocket ((player id / name) or (all / *)) (speed)";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float speed) && speed <= 0)
            {
                response = $"Speed argument should be a positive number : {arguments.At(1)}";
                return false;
            }

            foreach (Player ply in players)
                Timing.RunCoroutine(DoRocket(ply, speed));

            response = $"The specified players have been turned into fireworks\n{players.LogPlayers()}";
            return true;
        }
        public static IEnumerator<float> DoRocket(Player player, float speed)
        {
            const int maxAmnt = 50;
            int amnt = 0;
            while (player.IsAlive)
            {
                player.Position += Vector3.up * speed;
                amnt++;
                if (amnt >= maxAmnt)
                {
                    player.IsGodModeEnabled = false;
                    player.Explode();
                    player.Kill("Went on a trip in their favorite rocket ship.");
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
