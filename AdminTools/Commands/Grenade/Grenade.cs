using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Grenade
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Grenade : ParentCommand
    {
        public Grenade() => LoadGeneratedCommands();

        public override string Command { get; } = "grenade";

        public override string[] Aliases { get; } = new string[] { "gn" };

        public override string Description { get; } = "Spawns a frag/flash/scp018 grenade on a user or users";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.grenade"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2 || arguments.Count > 3)
            {
                response = "Usage: grenade ((player id / name) or (all / *)) (GrenadeType) (grenade time)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    if (!Enum.TryParse(arguments.At(1), true, out GrenadeType gType))
                    {
                        response = $"Invalid value for grenade name: {arguments.At(1)}";
                        return false;
                    }

                    if (gType == GrenadeType.Scp018)
                    {
                        Cassie.Message("pitch_1.5 xmas_bouncyballs", true, false);
                        foreach (Player pl in Player.List)
                        {
                            if (pl.Role == RoleType.Spectator || pl.Role == RoleType.None)
                                continue;

                            EventHandlers.SpawnGrenade(pl.Position, ItemType.SCP018,player: pl);
                        }
                    }
                    else
                    {
                        if (arguments.Count != 3)
                        {
                            response = "Usage: grenade ((player id / name) or (all / *)) (GrenadeType) (grenade time)";
                            return false;
                        }

                        if (!float.TryParse(arguments.At(2), out float time))
                        {
                            response = $"Invalid value for grenade timer: {arguments.At(2)}";
                            return false;
                        }

                        foreach (Player pl in Player.List)
                        {
                            if (pl.Role == RoleType.Spectator || pl.Role == RoleType.None)
                                continue;

                            if (gType == GrenadeType.Flashbang)
                                EventHandlers.SpawnGrenade(pl.Position, ItemType.GrenadeFlash, time,pl);
                            else
                                EventHandlers.SpawnGrenade(pl.Position, gType.GetItemType(), time, pl);
                        }
                    }
                    response = $"You spawned a {gType.ToString().ToLower()} on everyone";
                    return true;
                default:
                    Player ply = Player.Get(arguments.At(0));
                    if (ply == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }
                    else if (ply.Role == RoleType.Spectator || ply.Role == RoleType.None)
                    {
                        response = $"Player {ply.Nickname} is not a valid class to spawn a grenade on";
                        return false;
                    }

                    if (!Enum.TryParse(arguments.At(1), true, out GrenadeType type))
                    {
                        response = $"Invalid value for grenade name: {arguments.At(1)}";
                        return false;
                    }

                    if (type == GrenadeType.Scp018)
                        EventHandlers.SpawnGrenade(ply.Position, ItemType.SCP018,player: ply);
                    else
                    {
                        if (arguments.Count != 3)
                        {
                            response = "Usage: grenade ((player id / name) or (all / *)) (GrenadeType) (grenade time)";
                            return false;
                        }

                        if (!float.TryParse(arguments.At(2), out float time))
                        {
                            response = $"Invalid value for grenade timer: {arguments.At(2)}";
                            return false;
                        }
                        
                        if (type == GrenadeType.Flashbang)
                            EventHandlers.SpawnGrenade(ply.Position, ItemType.GrenadeFlash, time, ply);
                        else
                            EventHandlers.SpawnGrenade(ply.Position, type.GetItemType(), time, ply);
                    }

                    response = $"You spawned a {type.ToString().ToLower()} on {ply.Nickname}";
                    return true;
            }
        }
    }
}
