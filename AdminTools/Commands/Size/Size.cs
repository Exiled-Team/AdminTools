namespace AdminTools.Commands.Size
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using Extensions;
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    internal class Size : ParentCommand
    {
        public Size()
        {
            LoadGeneratedCommands();
        }

        public override string Command => "size";

        public override string[] Aliases => null;

        public override string Description => "Sets the size of all users or a user";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender,
            out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.size"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\nsize (player id / name) or (all / *)) (x value) (y value) (z value)" +
                    "\nsize reset";
                return false;
            }

            switch (arguments.At(0))
            {
                case "reset":
                    foreach (Player ply in Player.List)
                    {
                        if (ply.Role == RoleTypeId.Spectator || ply.Role == RoleTypeId.None)
                            continue;

                        ply.SetPlayerScale(1, 1, 1);
                    }

                    response = "Everyone's size has been reset";
                    return true;
                case "*":
                case "all":
                    if (arguments.Count != 4)
                    {
                        response = "Usage: size (all / *) (x) (y) (z)";
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

                    foreach (Player ply in Player.List)
                    {
                        if (ply.Role == RoleTypeId.Spectator || ply.Role == RoleTypeId.None)
                            continue;

                        ply.SetPlayerScale(xval, yval, zval);
                    }

                    response = $"Everyone's scale has been set to {xval} {yval} {zval}";
                    return true;
                default:
                    if (arguments.Count != 4)
                    {
                        response = "Usage: size (player id / name) (x) (y) (z)";
                        return false;
                    }

                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
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

                    pl.SetPlayerScale(x, y, z);
                    response = $"Player {pl.Nickname}'s scale has been set to {x} {y} {z}";
                    return true;
            }
        }
    }
}