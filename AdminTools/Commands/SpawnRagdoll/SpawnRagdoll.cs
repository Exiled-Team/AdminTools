namespace AdminTools.Commands.SpawnRagdoll
{
    using System;
    using System.Collections.Generic;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Permissions.Extensions;
    using MEC;
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class SpawnRagdoll : ICommand
    {
        public string Command => "spawnragdoll";

        public string[] Aliases { get; } = { "ragdoll", "rd", "rag", "doll" };

        public string Description => "Spawns a specified number of ragdolls on a user";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.dolls"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 3)
            {
                response = "Usage: spawnragdoll ((player id / name) or (all / *)) (RoleTypeId) (amount)";
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

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    foreach (Player player in Player.List)
                    {
                        if (player.Role != RoleTypeId.Spectator)
                            Timing.RunCoroutine(SpawnDolls(player, type, amount));
                    }

                    break;
                default:
                    Player ply = Player.Get(arguments.At(0));
                    if (ply is null)
                    {
                        response = $"Player {arguments.At(0)} not found.";
                        return false;
                    }

                    Timing.RunCoroutine(SpawnDolls(ply, type, amount));

                    break;
            }

            response = $"{amount} {type} ragdoll(s) have been spawned on {arguments.At(0)}.";
            return true;
        }

        private static IEnumerator<float> SpawnDolls(IPosition player, RoleTypeId type, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Ragdoll.CreateAndSpawn(type, "SCP-343", "End of the Universe", player.Position, default);
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}