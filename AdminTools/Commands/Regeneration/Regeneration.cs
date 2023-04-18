using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using System;
using AdminTools.Components;

namespace AdminTools.Commands.Regeneration
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Regeneration : ParentCommand
    {
        public Regeneration() => LoadGeneratedCommands();

        public override string Command { get; } = "reg";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Manages regeneration properties for users";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.reg"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\nreg ((player id / name) or (all / *)) ((doors) or (all))" +
                    "\nreg clear" +
                    "\nreg list" +
                    "\nreg health (value)" +
                    "\nreg time (value)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "clear":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: reg clear";
                        return false;
                    }

                    foreach (Player ply in Plugin.RegeneratingHubs.Keys)
                        if (ply.ReferenceHub.TryGetComponent(out RegenerationComponent rgCom))
                            UnityEngine.Object.Destroy(rgCom);

                    response = "Regeneration has been removed from everyone";
                    return true;
                case "list":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: regen list";
                        return false;
                    }

                    var playerLister = StringBuilderPool.Shared.Rent(Plugin.RegeneratingHubs.Count != 0 ? "Players with regeneration on:\n" : "No players currently online have regeneration on");
                    if (Plugin.RegeneratingHubs.Count == 0)
                    {
                        response = playerLister.ToString();
                        return true;
                    }

                    foreach (Player ply in Plugin.RegeneratingHubs.Keys)
                    {
                        playerLister.Append(ply.Nickname);
                        playerLister.Append(", ");
                    }

                    var msg = playerLister.ToString().Substring(0, playerLister.ToString().Length - 2);
                    StringBuilderPool.Shared.Return(playerLister);
                    response = msg;
                    return true;
                case "heal":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: reg heal (value)";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(1), out var healvalue) || healvalue < 0.05)
                    {
                        response = $"Invalid value for healing: {arguments.At(1)}";
                        return false;
                    }

                    Plugin.HealthGain = healvalue;
                    response = $"Players with regeneration will heal {healvalue} HP per interval";
                    return true;
                case "time":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: reg time (value)";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(1), out var healtime) || healtime < 0.05)
                    {
                        response = $"Invalid value for healing time interval: {arguments.At(1)}";
                        return false;
                    }

                    Plugin.HealthInterval = healtime;
                    response = $"Players with regeneration will heal every {healtime} seconds";
                    return true;
                case "*":
                case "all":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: reg (all / *)";
                        return false;
                    }

                    foreach (var ply in Player.List)
                        if (!ply.ReferenceHub.TryGetComponent(out RegenerationComponent _))
                            ply.ReferenceHub.gameObject.AddComponent<RegenerationComponent>();

                    response = "Everyone on the server can regenerate health now";
                    return true;
                default:
                    if (arguments.Count != 1)
                    {
                        response = "Usage: reg (player id / name)";
                        return false;
                    }

                    var pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!pl.ReferenceHub.TryGetComponent(out RegenerationComponent rgnComponent))
                    {
                        pl.GameObject.AddComponent<RegenerationComponent>();
                        response = $"Regeneration is on for {pl.Nickname}";
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(rgnComponent);
                        response = $"Regeneration is off for {pl.Nickname}";
                    }
                    return true;
            }
        }
    }
}