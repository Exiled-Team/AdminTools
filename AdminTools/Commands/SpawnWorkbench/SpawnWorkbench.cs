using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdminTools.Commands.SpawnWorkbench
{
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnWorkbench : ParentCommand
    {
        public SpawnWorkbench() => LoadGeneratedCommands();

        public override string Command { get; } = "bench";

        public override string[] Aliases { get; } = new string[] { "sw", "wb", "workbench" };

        public override string Description { get; } = "Spawns a workbench on all users or a user";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.benches"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (!(sender is PlayerCommandSender plysend))
            {
                response = "You must be in-game to run this command!";
                return false;
            }

            Player player = Player.Get(plysend.ReferenceHub);

            if (arguments.Count < 1)
            {
                response = "Usage: bench ((player id / name) or (all / *)) (x value) (y value) (z value)" +
                    "\nbench clear (player id / name) (minimum index) (maximum index)" +
                    "\nbench clearall" +
                    "\nbench count (player id / name)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "clear":
                    if (arguments.Count != 4)
                    {
                        response = "Usage:\nbench clear (player id / name) (minimum index) (maximum index)\n\nNOTE: Minimum index < Maximum index, You can remove from a range of all the benches you spawned (From 1 to (how many you spawned))";
                        return false;
                    }

                    Player ply = Player.Get(arguments.At(1));
                    if (ply == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (!int.TryParse(arguments.At(2), out int min) && min < 0)
                    {
                        response = $"Invalid value for minimum index: {arguments.At(2)}";
                        return false;
                    }

                    if (!int.TryParse(arguments.At(3), out int max) && max < 0)
                    {
                        response = $"Invalid value for maximum index: {arguments.At(3)}";
                        return false;
                    }

                    if (max < min)
                    {
                        response = $"{max} is not greater than {min}";
                        return false;
                    }

                    if (!Plugin.SpawnedBenchHubs.TryGetValue(ply, out List<GameObject> objs))
                    {
                        response = $"{ply.Nickname} has not spawned in any workbenches";
                        return false;
                    }

                    if (min > objs.Count)
                    {
                        response = $"{min} (minimum) is higher than the number of workbenches {ply.Nickname} spawned! (Which is {objs.Count})";
                        return false;
                    }

                    if (max > objs.Count)
                    {
                        response = $"{max} (maximum) is higher than the number of workbenches {ply.Nickname} spawned! (Which is {objs.Count})";
                        return false;
                    }

                    min = min == 0 ? 0 : min - 1;
                    max = max == 0 ? 0 : max - 1;

                    for (int i = min; i <= max; i++)
                    {
                        UnityEngine.Object.Destroy(objs.ElementAt(i));
                        objs[i] = null;
                    }
                    objs.RemoveAll(r => r == null);

                    response = $"All workbenches from {min + 1} to {max + 1} have been cleared from Player {ply.Nickname}";
                    return true;
                case "clearall":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: bench clearall";
                        return true;
                    }

                    foreach (KeyValuePair<Player, List<GameObject>> bch in Plugin.SpawnedBenchHubs)
                    {
                        foreach (GameObject bench in bch.Value)
                            UnityEngine.Object.Destroy(bench);
                        bch.Value.Clear();
                    }

                    Plugin.SpawnedBenchHubs.Clear();
                    response = $"All spawned workbenches have now been removed";
                    return true;
                case "count":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: bench count (player id / name)";
                        return false;
                    }

                    Player plyr = Player.Get(arguments.At(1));
                    if (plyr == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (!Plugin.SpawnedBenchHubs.TryGetValue(plyr, out List<GameObject> obj) || obj.Count == 0)
                    {
                        response = $"{plyr.Nickname} has not spawned in any workbenches";
                        return false;
                    }

                    response = $"{plyr.Nickname} has spawned in {(obj.Count != 1 ? $"{obj.Count} workbenches" : $"{obj.Count} workbench")}";
                    return true;
                case "*":
                case "all":
                    if (arguments.Count != 4)
                    {
                        response = "Usage: bench (all / *) (x value) (y value) (z value)";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(1), out float xval))
                    {
                        response = $"Invalid value for x size: {arguments.At(1)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(2), out float yval))
                    {
                        response = $"Invalid value for y size: {arguments.At(2)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(3), out float zval))
                    {
                        response = $"Invalid value for z size: {arguments.At(3)}";
                        return false;
                    }

                    int index = 0;
                    foreach (Player p in Player.List)
                    {
                        if (p.Role == RoleTypeId.Spectator || p.Role == RoleTypeId.None)
                            continue;

                        EventHandlers.SpawnWorkbench(player, p.Position + p.ReferenceHub.PlayerCameraReference.forward * 2, p.GameObject.transform.rotation.eulerAngles, new Vector3(xval, yval, zval), out int benchIndex);
                        index = benchIndex;
                    }

                    response = $"A workbench has spawned on everyone, you now spawned in a total of {(index != 1 ? $"{index} workbenches" : $"{index} workbench")}";
                    return true;
                default:
                    if (arguments.Count != 4)
                    {
                        response = "Usage: bench (player id / name) (x value) (y value) (z value)";
                        return false;
                    }

                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }
                    else if (pl.Role == RoleTypeId.Spectator || pl.Role == RoleTypeId.None)
                    {
                        response = $"This player is not a valid class to spawn a workbench on";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(1), out float x))
                    {
                        response = $"Invalid value for x size: {arguments.At(1)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(2), out float y))
                    {
                        response = $"Invalid value for y size: {arguments.At(2)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(3), out float z))
                    {
                        response = $"Invalid value for z size: {arguments.At(3)}";
                        return false;
                    }

                    EventHandlers.SpawnWorkbench(player, pl.Position + pl.ReferenceHub.PlayerCameraReference.forward * 2, pl.GameObject.transform.rotation.eulerAngles, new Vector3(x, y, z), out int benchI);
                    response = $"A workbench has spawned on Player {pl.Nickname}, you now spawned in a total of {(benchI != 1 ? $"{benchI} workbenches" : $"{benchI} workbench")}";
                    return true;
            }
        }
    }
}
