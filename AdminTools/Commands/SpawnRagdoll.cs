using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using System;
using System.Collections.Generic;
using PlayerRoles;
using System.Text.RegularExpressions;
using System.Linq;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class SpawnRagdoll : ICommand, IUsageProvider
    {
        public string Command { get; } = "spawnragdoll";

        public string[] Aliases { get; } = new string[] { "ragdoll", "rd", "rag", "doll" };

        public string Description { get; } = "Spawns a specified number of ragdolls on a user";

        public string[] Usage { get; } = new string[] { "%player%", "%role%", "amount", "\"name\"", "\"death reason\"" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string[] quotedArgs = Regex.Matches(string.Join(" ", arguments), "[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray()
                .ToArray();

            if (!sender.CheckPermission("at.dolls"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (quotedArgs.Count() != 5)
            {
                response = "Usage: spawnragdoll ((player id / name) or (all / *)) (RoleTypeId) (amount) (nameRagdoll) (deathReason)";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out RoleTypeId type))
            {
                response = $"Invalid RoleTypeId for ragdoll: {arguments.At(1)}";
                return false;
            }

            if (!int.TryParse(arguments.At(2), out int amount))
            {
                response = $"Invalid amount of ragdolls to spawn: {arguments.At(2)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            string ragdollName = quotedArgs.ElementAt(3).Trim('\"');
            string deathReason = quotedArgs.ElementAt(4).Trim('\"');
            foreach (Player player in players)
            {
                Timing.RunCoroutine(SpawnDolls(player, type, amount, ragdollName, deathReason));
            }

            response = $"{amount} {type} ragdoll(s) have been spawned on {arguments.At(0)}.";
            return true;
        }

        private IEnumerator<float> SpawnDolls(Player player, RoleTypeId type, int amount, string ragdollName, string deathReason)
        {
            for (int i = 0; i < amount; i++)
            {
                if (player.IsAlive)
                    Ragdoll.CreateAndSpawn(type, ragdollName, deathReason, player.Position, default);
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}
