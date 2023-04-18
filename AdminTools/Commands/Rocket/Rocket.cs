namespace AdminTools.Commands.Rocket
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using MEC;
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Rocket : ICommand
    {
        public string Command => "rocket";

        public string[] Aliases => null;

        public string Description => "Sends players high in the sky and explodes them";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
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
                    if (!float.TryParse(arguments.At(1), out float speed) && speed <= 0)
                    {
                        response = $"Speed argument invalid: {arguments.At(1)}";
                        return false;
                    }

                    foreach (Player ply in Player.List)
                    {
                        Timing.RunCoroutine(API.Rocket.DoRocket(ply, speed));
                    }

                    response =
                        "Everyone has been rocketed into the sky (We're going on a trip, in our favorite rocketship)";
                    return true;
                default:
                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (pl.Role == RoleTypeId.Spectator || pl.Role == RoleTypeId.None)
                    {
                        response = $"Player {pl.Nickname} is not a valid class to rocket";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(1), out float spd) && spd <= 0)
                    {
                        response = $"Speed argument invalid: {arguments.At(1)}";
                        return false;
                    }

                    Timing.RunCoroutine(API.Rocket.DoRocket(pl, spd));

                    response =
                        $"Player {pl.Nickname} has been rocketed into the sky (We're going on a trip, in our favorite rocketship)";
                    return true;
            }
        }
    }
}