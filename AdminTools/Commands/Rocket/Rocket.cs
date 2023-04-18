using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using System;

namespace AdminTools.Commands.Rocket
{
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Rocket : ParentCommand
    {
        public Rocket() => LoadGeneratedCommands();

        public override string Command { get; } = "rocket";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Sends players high in the sky and explodes them";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.rocket"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 2)
            {
                response = "Usage: rocket ((player id / name) or (all / *)) (speed)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    if (!float.TryParse(arguments.At(1), out var speed) && speed <= 0)
                    {
                        response = $"Speed argument invalid: {arguments.At(1)}";
                        return false;
                    }

                    foreach (var ply in Player.List)
                        Timing.RunCoroutine(EventHandlers.DoRocket(ply, speed));

                    response = "Everyone has been rocketed into the sky (We're going on a trip, in our favorite rocketship)";
                    return true;
                default:
                    var pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }
                    else if (pl.Role == RoleTypeId.Spectator || pl.Role == RoleTypeId.None)
                    {
                        response = $"Player {pl.Nickname} is not a valid class to rocket";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(1), out var spd) && spd <= 0)
                    {
                        response = $"Speed argument invalid: {arguments.At(1)}";
                        return false;
                    }

                    Timing.RunCoroutine(EventHandlers.DoRocket(pl, spd));
                    response = $"Player {pl.Nickname} has been rocketed into the sky (We're going on a trip, in our favorite rocketship)";
                    return true;
            }
        }
    }
}
