namespace AdminTools.Commands.Position
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using API.Enums;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using NorthwoodLib.Pools;
    using UnityEngine;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Position : ICommand
    {
        public string Command => "positon";

        public string[] Aliases { get; } = { "pos" };

        public string Description => "Modifies or retrieves the position of a user or all users";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.tp"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = response =
                    "\nUsage:\nposition ((player id / name) or (all / *)) (set) (x position) (y position) (z position)\nposition ((player id / name) or (all / *)) (get)\nposition ((player id / name) or (all / *))(add) (x, y, or z) (value)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    if (!Enum.TryParse(arguments.At(1), true, out PositionModifier mod))
                    {
                        response = $"Invalid position modifier: {arguments.At(0)}";
                        return false;
                    }

                    switch (mod)
                    {
                        case PositionModifier.Set:
                            if (arguments.Count != 5)
                            {
                                response = "Usage: position (all / *) (set) (x position) (y position) (z position)";
                                return false;
                            }

                            if (!float.TryParse(arguments.At(2), out float xval))
                            {
                                response = $"Invalid value for x position: {arguments.At(2)}";
                                return false;
                            }

                            if (!float.TryParse(arguments.At(3), out float yval))
                            {
                                response = $"Invalid value for x position: {arguments.At(3)}";
                                return false;
                            }

                            if (!float.TryParse(arguments.At(4), out float zval))
                            {
                                response = $"Invalid value for x position: {arguments.At(4)}";
                                return false;
                            }

                            if (Player.List.Count() == 0)
                            {
                                response = "There are no players currently online";
                                return true;
                            }

                            foreach (Player ply in Player.List)
                            {
                                ply.Position = new Vector3(xval, yval, zval);
                            }

                            response = $"All player's positions have been set to {xval} {yval} {zval}";
                            return true;

                        case PositionModifier.Get:
                            if (arguments.Count != 2)
                            {
                                response = "Usage: position (all / *) (get)";
                                return false;
                            }

                            StringBuilder positionBuilder = StringBuilderPool.Shared.Rent();
                            if (Player.List.Count() == 0)
                            {
                                response = "There are no players currently online";
                                return true;
                            }

                            positionBuilder.Append("\n");
                            foreach (Player ply in Player.List)
                            {
                                positionBuilder.Append(ply.Nickname);
                                positionBuilder.Append("'s (");
                                positionBuilder.Append(ply.Id);
                                positionBuilder.Append(")");
                                positionBuilder.Append(" position: ");
                                positionBuilder.Append(ply.Position.x);
                                positionBuilder.Append(" ");
                                positionBuilder.Append(ply.Position.y);
                                positionBuilder.Append(" ");
                                positionBuilder.AppendLine(ply.Position.z.ToString(CultureInfo.InvariantCulture));
                            }

                            string message = positionBuilder.ToString();
                            StringBuilderPool.Shared.Return(positionBuilder);
                            response = message;
                            return true;
                        case PositionModifier.Add:
                            if (arguments.Count != 4)
                            {
                                response = "Usage: position (all / *) (add) (x, y, or z) (value)";
                                return false;
                            }

                            if (!Enum.TryParse(arguments.At(2), true, out VectorAxis axis))
                            {
                                response = $"Invalid value for vector axis: {arguments.At(2)}";
                                return false;
                            }

                            if (!float.TryParse(arguments.At(3), out float val))
                            {
                                response = $"Invalid value for position: {arguments.At(3)}";
                                return false;
                            }

                            switch (axis)
                            {
                                case VectorAxis.X:
                                    foreach (Player ply in Player.List)
                                    {
                                        ply.Position = new Vector3(ply.Position.x + val, ply.Position.y,
                                            ply.Position.z);
                                    }

                                    response = $"Every player's x position has been added by {val}";
                                    return true;
                                case VectorAxis.Y:
                                    foreach (Player ply in Player.List)
                                    {
                                        ply.Position = new Vector3(ply.Position.x, ply.Position.y + val,
                                            ply.Position.z);
                                    }

                                    response = $"Every player's y position has been added by {val}";
                                    return true;
                                case VectorAxis.Z:
                                    foreach (Player ply in Player.List)
                                    {
                                        ply.Position = new Vector3(ply.Position.x, ply.Position.y,
                                            ply.Position.z + val);
                                    }

                                    response = $"Every player's z position has been added by {val}";
                                    return true;
                            }

                            break;
                        default:
                            response =
                                "\nUsage:\nposition (all / *) (set) (x position) (y position) (z position)\nposition (all / *) (get)\nposition (all / *) (add) (x, y, or z) (value)";
                            return false;
                    }

                    break;
                default:
                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!Enum.TryParse(arguments.At(1), true, out PositionModifier modf))
                    {
                        response = $"Invalid position modifier: {arguments.At(1)}";
                        return false;
                    }

                    switch (modf)
                    {
                        case PositionModifier.Set:
                            if (arguments.Count != 5)
                            {
                                response =
                                    "Usage: position (player id / name) (set) (x position) (y position) (z position)";
                                return false;
                            }

                            if (!float.TryParse(arguments.At(2), out float xval))
                            {
                                response = $"Invalid value for x position: {arguments.At(2)}";
                                return false;
                            }

                            if (!float.TryParse(arguments.At(3), out float yval))
                            {
                                response = $"Invalid value for x position: {arguments.At(3)}";
                                return false;
                            }

                            if (!float.TryParse(arguments.At(4), out float zval))
                            {
                                response = $"Invalid value for x position: {arguments.At(4)}";
                                return false;
                            }

                            pl.Position = new Vector3(xval, yval, zval);

                            response = $"Player {pl.Nickname}'s positions have been set to {xval} {yval} {zval}";
                            return true;
                        case PositionModifier.Get:
                            if (arguments.Count != 2)
                            {
                                response = "Usage: position (player id / name) (get)";
                                return false;
                            }

                            response =
                                $"Player {pl.Nickname}'s ({pl.Id}) position is {pl.Position.x} {pl.Position.y} {pl.Position.z}";
                            return true;
                        case PositionModifier.Add:
                            if (arguments.Count != 4)
                            {
                                response = "Usage: position (player id / name) (add) (x, y, or z) (value)";
                                return false;
                            }

                            if (!Enum.TryParse(arguments.At(2), true, out VectorAxis axis))
                            {
                                response = $"Invalid value for vector axis: {arguments.At(2)}";
                                return false;
                            }

                            if (!float.TryParse(arguments.At(3), out float val))
                            {
                                response = $"Invalid value for position: {arguments.At(2)}";
                                return false;
                            }

                            switch (axis)
                            {
                                case VectorAxis.X:
                                    pl.Position = new Vector3(pl.Position.x + val, pl.Position.y, pl.Position.z);
                                    response = $"Player {pl.Nickname}'s x position has been added by {val}";
                                    return true;
                                case VectorAxis.Y:
                                    pl.Position = new Vector3(pl.Position.x, pl.Position.y + val, pl.Position.z);
                                    response = $"Player {pl.Nickname}'s y position has been added by {val}";
                                    return true;
                                case VectorAxis.Z:
                                    pl.Position = new Vector3(pl.Position.x, pl.Position.y, pl.Position.z + val);
                                    response = $"Player {pl.Nickname}'s z position has been added by {val}";
                                    return true;
                            }

                            break;
                        default:
                            response =
                                "\nUsage:\nposition (player id / name) (set) (x position) (y position) (z position)\nposition (player id / name) (get)\nposition (player id / name) (add) (x, y, or z) (value)";
                            return false;
                    }

                    break;
            }

            response = "Something did not go right, the command should not reach this point";
            return false;
        }
    }
}