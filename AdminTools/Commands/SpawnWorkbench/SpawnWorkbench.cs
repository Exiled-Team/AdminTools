using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdminTools.Commands.SpawnWorkbench
{
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
            response = "Command broken after release. Will be fixed soonTM!";
            return false;

            EventHandlers.LogCommandUsed((CommandSender)sender, EventHandlers.FormatArguments(arguments, 0));
            if (!((CommandSender)sender).CheckPermission("at.benches"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            Player Sender = Player.Get(((CommandSender)sender).ReferenceHub);
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

                    Player Ply = Player.Get(arguments.At(1));
                    if (Ply == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (!int.TryParse(arguments.At(2), out int Min) && Min < 0)
                    {
                        response = $"Invalid value for minimum index: {arguments.At(2)}";
                        return false;
                    }

                    if (!int.TryParse(arguments.At(3), out int Max) && Max < 0)
                    {
                        response = $"Invalid value for maximum index: {arguments.At(3)}";
                        return false;
                    }

                    if (Max < Min)
                    {
                        response = $"{Max} is not greater than {Min}";
                        return false;
                    }

                    if (!Plugin.BchHubs.TryGetValue(Ply, out List<GameObject> objs))
                    {
                        response = $"{Ply.Nickname} has not spawned in any workbenches";
                        return false;
                    }

                    if (Min > objs.Count)
                    {
                        response = $"{Min} (minimum) is higher than the number of workbenches {Ply.Nickname} spawned! (Which is {objs.Count})";
                        return false;
                    }

                    if (Max > objs.Count)
                    {
                        response = $"{Max} (maximum) is higher than the number of workbenches {Ply.Nickname} spawned! (Which is {objs.Count})";
                        return false;
                    }

                    Min = Min == 0 ? 0 : Min - 1;
                    Max = Max == 0 ? 0 : Max - 1;

                    for (int i = Min; i <= Max; i++)
                    {
                        UnityEngine.Object.Destroy(objs.ElementAt(i));
                        objs[i] = null;
                    }
                    objs.RemoveAll(r => r == null);

                    response = $"All workbenches from {Min + 1} to {Max + 1} have been cleared from Player {Ply.Nickname}";
                    return true;
                case "clearall":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: bench clearall";
                        return true;
                    }

                    foreach (KeyValuePair<Player, List<GameObject>> Bch in Plugin.BchHubs)
                    {
                        foreach (GameObject Bench in Bch.Value)
                            UnityEngine.Object.Destroy(Bench);
                        Bch.Value.Clear();
                    }

                    Plugin.BchHubs.Clear();
                    response = $"All spawned workbenches have now been removed";
                    return true;
                case "count":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: bench count (player id / name)";
                        return false;
                    }

                    Player Plyr = Player.Get(arguments.At(1));
                    if (Plyr == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (!Plugin.BchHubs.TryGetValue(Plyr, out List<GameObject> obj) || obj.Count == 0)
                    {
                        response = $"{Plyr.Nickname} has not spawned in any workbenches";
                        return false;
                    }

                    response = $"{Plyr.Nickname} has spawned in {(obj.Count != 1 ? $"{obj.Count} workbenches" : $"{obj.Count} workbench")}";
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

                    int Index = 0;
                    foreach (Player P in Player.List)
                    {
                        if (P.Role == RoleType.Spectator || P.Role == RoleType.None)
                            continue;

                        EventHandlers.SpawnWorkbench(Sender, P.Position + P.ReferenceHub.PlayerCameraReference.forward * 2, P.GameObject.transform.rotation.eulerAngles, new Vector3(xval, yval, zval), out int BenchIndex);
                        Index = BenchIndex;
                    }

                    response = $"A workbench has spawned on everyone, you now spawned in a total of {(Index != 1 ? $"{Index} workbenches" : $"{Index} workbench")}";
                    return true;
                default:
                    if (arguments.Count != 4)
                    {
                        response = "Usage: bench (player id / name) (x value) (y value) (z value)";
                        return false;
                    }

                    Player Pl = Player.Get(arguments.At(0));
                    if (Pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }
                    else if (Pl.Role == RoleType.Spectator || Pl.Role == RoleType.None)
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

                    EventHandlers.SpawnWorkbench(Sender, Pl.Position + Pl.ReferenceHub.PlayerCameraReference.forward * 2, Pl.GameObject.transform.rotation.eulerAngles, new Vector3(x, y, z), out int BenchI);
                    response = $"A workbench has spawned on Player {Pl.Nickname}, you now spawned in a total of {(BenchI != 1 ? $"{BenchI} workbenches" : $"{BenchI} workbench")}";
                    return true;
            }
        }
    }
}
