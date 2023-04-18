namespace AdminTools.Commands.TeleportX
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class TeleportX : ICommand
    {
        public string Command => "teleportx";

        public string[] Aliases { get; } = { "tpx" };

        public string Description => "Teleports all users or a user to another user";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.tp"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 2)
            {
                response =
                    "Usage: teleportx (People teleported: (player id / name) or (all / *)) (Teleported to: (player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    Player ply = Player.Get(arguments.At(1));
                    if (ply == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }


                    foreach (Player plyr in Player.List)
                    {
                        if (plyr.Role == RoleTypeId.Spectator || ply.Role == RoleTypeId.None)
                            continue;

                        plyr.Position = ply.Position;
                    }

                    response = $"Everyone has been teleported to Player {ply.Nickname}";
                    return true;
                default:
                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    Player plr = Player.Get(arguments.At(1));
                    if (plr == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    pl.Position = plr.Position;
                    response = $"Player {pl.Nickname} has been teleported to Player {plr.Nickname}";
                    return true;
            }
        }
    }
}