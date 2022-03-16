using System;
using System.Text;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;

namespace AdminTools.Commands.InfiniteAmmo
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class InfiniteAmmoCommand : ParentCommand
    {
                public InfiniteAmmoCommand() => LoadGeneratedCommands();

        public override string Command { get; } = "infiniteammo";

        public override string[] Aliases { get; } = new string[] { "infammo", "ia" };

        public override string Description { get; } = "Manage infinite ammo properties for users";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            EventHandlers.LogCommandUsed((CommandSender)sender, EventHandlers.FormatArguments(arguments, 0));
            if (!((CommandSender)sender).CheckPermission("at.ia"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\ninfiniteammo ((player id / name) or (all / *))" +
                    "\ninfiniteammo clear" +
                    "\ninfiniteammo list" +
                    "\ninfiniteammo remove (player id / name)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "clear":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: infiniteammo clear";
                        return false;
                    }

                    foreach (Player Ply in Plugin.IaHubs.Keys)
                        if (Ply.ReferenceHub.TryGetComponent(out InfiniteAmmoComponent IaCom))
                            UnityEngine.Object.Destroy(IaCom);

                    response = "Infinite ammo has been removed from everyone";
                    return true;
                case "list":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: infiniteammo clear";
                        return false;
                    }

                    StringBuilder PlayerLister = StringBuilderPool.Shared.Rent(Plugin.IaHubs.Count != 0 ? "Players with infinite ammo on:\n" : "No players currently online have infinite ammo on");
                    if (Plugin.IaHubs.Count == 0)
                    {
                        response = PlayerLister.ToString();
                        return true;
                    }

                    foreach (Player Ply in Plugin.IaHubs.Keys)
                    {
                        PlayerLister.Append(Ply.Nickname);
                        PlayerLister.Append(", ");
                    }

                    string msg = PlayerLister.ToString().Substring(0, PlayerLister.ToString().Length - 2);
                    StringBuilderPool.Shared.Return(PlayerLister);
                    response = msg;
                    return true;
                case "remove":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: infiniteammo remove (player id / name)";
                        return false;
                    }

                    Player Pl = Player.Get(arguments.At(1));
                    if (Pl == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (Pl.ReferenceHub.TryGetComponent(out InfiniteAmmoComponent IaComponent))
                    {
                        Plugin.IaHubs.Remove(Pl);
                        UnityEngine.Object.Destroy(IaComponent);
                        response = $"Infinite ammo is off for {Pl.Nickname} now";
                    }
                    else
                        response = $"Player {Pl.Nickname} does not have the infinite ammo ability";
                    return true;
                case "*":
                case "all":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: infiniteammo all / *";
                        return false;
                    }

                    foreach (Player Ply in Player.List)
                        if (!Ply.ReferenceHub.TryGetComponent(out InfiniteAmmoComponent _))
                            Ply.ReferenceHub.gameObject.AddComponent<InfiniteAmmoComponent>();

                    response = "Everyone on the server have infinite ammo now";
                    return true;
                default:
                    if (arguments.Count != 1)
                    {
                        response = "Usage: infiniteammo (player id / name)";
                        return false;
                    }

                    Player Plyr = Player.Get(arguments.At(0));
                    if (Plyr == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!Plyr.ReferenceHub.TryGetComponent(out InfiniteAmmoComponent iaComponent))
                    {
                        Plyr.GameObject.AddComponent<InfiniteAmmoComponent>();
                        response = $"Infinite ammo is on for {Plyr.Nickname}";
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(iaComponent);
                        response = $"Infinite ammo is off for {Plyr.Nickname}";
                    }
                    return true;
            }
        }
    }
}