using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdminTools.Commands.Dummy
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Dummy : ParentCommand
    {
        public Dummy() => LoadGeneratedCommands();

        public override string Command { get; } = "dummy";

        public override string[] Aliases { get; } = new string[] { "dum" };

        public override string Description { get; } = "Spawns a dummy character on all users on a user";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            EventHandlers.LogCommandUsed((CommandSender)sender, EventHandlers.FormatArguments(arguments, 0));
            if (!((CommandSender)sender).CheckPermission("at.dummy"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            Player Sender = Player.Get(((CommandSender)sender).ReferenceHub);
            if (arguments.Count < 1)
            {
                response = "Usage:\ndummy ((player id / name) or (all / *)) (RoleType) (x value) (y value) (z value)" +
                    "\ndummy clear (player id / name) (minimum index) (maximum index)" +
                    "\ndummy clearall" +
                    "\ndummy count (player id / name) ";
                return false;
            }

            switch (arguments.At(0))
            {
                case "clear":
                    if (arguments.Count != 4)
                    {
                        response = "Usage: dummy clear (player id / name) (minimum index) (maximum index)\nNote: Minimum < Maximum, you can remove from a range of dummies a user spawns";
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

                    if (!Plugin.DumHubs.TryGetValue(Ply, out List<GameObject> objs))
                    {
                        response = $"{Ply.Nickname} has not spawned in any dummies in";
                        return false;
                    }

                    if (Min > objs.Count)
                    {
                        response = $"{Min} (minimum) is higher than the number of dummies {Ply.Nickname} spawned! (Which is {objs.Count})";
                        return false;
                    }

                    if (Max > objs.Count)
                    {
                        response = $"{Max} (maximum) is higher than the number of dummies {Ply.Nickname} spawned! (Which is {objs.Count})";
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

                    response = $"All dummies from {Min + 1} to {Max + 1} have been cleared from Player {Ply.Nickname}";
                    return true;
                case "clearall":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: dummy clearall";
                        return false;
                    }

                    foreach (KeyValuePair<Player, List<GameObject>> Dummy in Plugin.DumHubs)
                    {
                        foreach (GameObject Dum in Dummy.Value)
                            UnityEngine.Object.Destroy(Dum);
                        Dummy.Value.Clear();
                    }

                    Plugin.DumHubs.Clear();
                    response = $"All spawned dummies have now been removed";
                    return true;
                case "count":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: dummy count (player id / name)";
                        return false;
                    }

                    Player Plyr = Player.Get(arguments.At(1));
                    if (Plyr == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (!Plugin.DumHubs.TryGetValue(Plyr, out List<GameObject> obj) || obj.Count == 0)
                    {
                        response = $"{Plyr.Nickname} has not spawned in any dummies in";
                        return false;
                    }

                    response = $"{Plyr.Nickname} has spawned in {(obj.Count != 1 ? $"{obj.Count} dummies" : $"{obj.Count} dummy")}";
                    return true;
                case "*":
                case "all":
                    if (arguments.Count != 5)
                    {
                        response = "Usage: dummy (all / *) (RoleType) (x value) (y value) (z value)";
                        return false;
                    }

                    if (!Enum.TryParse(arguments.At(1), true, out RoleType Role))
                    {
                        response = $"Invalid value for role type: {arguments.At(1)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(2), out float xval))
                    {
                        response = $"Invalid x value for dummy size: {arguments.At(2)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(3), out float yval))
                    {
                        response = $"Invalid y value for dummy size: {arguments.At(3)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(4), out float zval))
                    {
                        response = $"Invalid z value for dummy size: {arguments.At(4)}";
                        return false;
                    }
                    int Index = 0;
                    foreach (Player P in Player.List)
                    {
                        if (P.Role == RoleType.Spectator || P.Role == RoleType.None)
                            continue;

                        EventHandlers.SpawnDummyModel(Sender, P.Position, P.GameObject.transform.localRotation, Role, xval, yval, zval, out int DIndex);
                        Index = DIndex;
                    }

                    response = $"A {Role.ToString()} dummy has spawned on everyone, you now spawned in a total of {(Index != 1 ? $"{Index} dummies" : $"{Index} dummies")}";
                    return true;
                default:
                    if (arguments.Count != 5)
                    {
                        response = "Usage: dummy (player id / name) (RoleType) (x value) (y value) (z value)";
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
                        response = $"This player is not a valid class to spawn a dummy on";
                        return false;
                    }

                    if (!Enum.TryParse(arguments.At(1), true, out RoleType R))
                    {
                        response = $"Invalid value for role type: {arguments.At(1)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(2), out float x))
                    {
                        response = $"Invalid x value for dummy size: {arguments.At(2)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(3), out float y))
                    {
                        response = $"Invalid y value for dummy size: {arguments.At(3)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(4), out float z))
                    {
                        response = $"Invalid z value for dummy size: {arguments.At(4)}";
                        return false;
                    }

                    EventHandlers.SpawnDummyModel(Sender, Pl.Position, Pl.GameObject.transform.localRotation, R, x, y, z, out int DummyIndex);
                    response = $"A {R.ToString()} dummy has spawned on Player {Pl.Nickname}, you now spawned in a total of {(DummyIndex != 1 ? $"{DummyIndex} dummies" : $"{DummyIndex} dummy")}";
                    return true;
            }
        }
    }
}
