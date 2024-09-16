using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using Respawning;
using Respawning.NamingRules;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class FakeSpawnCommand : ICommand, IUsageProvider
    {
        public string Command { get; } = "fakespawn";

        public string[] Aliases { get; } = { "fakesync", "fakerole" };

        public string Description { get; } = "Sets everyone or a specific user to be invisible";

        public string[] Usage { get; } = { "%player%", "%player%", "%role%" ,"[id]"};

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.ghost"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: fakespawn ((player id / name) or (all / *)) ((player id / name) or (all / *)) (RoleTypeId) [id]";
                return false;
            }

            if (!Enum.TryParse(arguments.At(2), true, out RoleTypeId roletype))
            {
                response = $"Invalid value for RoleTypeId: {arguments.At(2)}\n{string.Join(", ", Enum.GetNames(typeof(RoleTypeId)))}.";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            IEnumerable<Player> playersToAffect = Player.GetProcessedData(arguments, 1);
            if (playersToAffect.IsEmpty())
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }
            byte id = (byte)(UnitNameMessageHandler.ReceivedNames.Count - 1);
            if (roletype.TryGetAssignedSpawnableTeam(out SpawnableTeamType _) && byte.TryParse(arguments.ElementAtOrDefault(2), out id) && UnitNameMessageHandler.ReceivedNames.Count > id)
                id = (byte)(UnitNameMessageHandler.ReceivedNames.Count - 1);

            foreach (Player player in players)
                player.ChangeAppearance(roletype, playersToAffect, false, id);

            response = $"The following players have been changed to '{roletype}':\n{players.LogPlayers()}";
            return true;
        }
    }
}
