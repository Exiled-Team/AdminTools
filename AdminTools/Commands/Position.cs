using System;
using System.Collections.Generic;
using System.Text;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using UnityEngine;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Position : ICommand, IUsageProvider
    {
        public string Command { get; } = "position";

        public string[] Aliases { get; } = { "pos" };

        public string Description { get; } = "Modifies or retrieves the position of a user or all users";

        public string[] Usage { get; } = { "%player%", string.Join(", ", Enum.GetNames(typeof(PositionModifier))), };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response) // TODO: Make it ParentCommand
        {
            if (!sender.CheckPermission("at.tp"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = response = "\nUsage:\nposition ((player id / name) or (all / *)) (set) (x position) (y position) (z position)\nposition ((player id / name) or (all / *)) (get)\nposition ((player id / name) or (all / *))(add) (x, y, or z) (value)";
                return false;
            }
            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }
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
                        response = $"Invalid value for y position: {arguments.At(3)}";
                        return false;
                    }
                    if (!float.TryParse(arguments.At(4), out float zval))
                    {
                        response = $"Invalid value for z position: {arguments.At(4)}";
                        return false;
                    }
                    if (players.IsEmpty())
                    {
                        response = "There are no players found";
                        return false;
                    }
                    foreach (Player ply in players)
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
                    if (players.IsEmpty())
                    {
                        response = "No players match your query.";
                        return false;
                    }
                    positionBuilder.Append("\n");
                    foreach (Player ply in players)
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
                        positionBuilder.AppendLine(ply.Position.z.ToString());
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
                        response = $"Invalid value for vector axis ({string.Join(", ", Enum.GetNames(typeof(PositionModifier)))}): {arguments.At(2)}";
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
                            foreach (Player ply in players)
                                ply.Position = new Vector3(ply.Position.x + val, ply.Position.y, ply.Position.z);

                            response = $"Every player's x position has been nudged by {val}";
                            return true;
                        case VectorAxis.Y:
                            foreach (Player ply in players)
                                ply.Position = new Vector3(ply.Position.x, ply.Position.y + val, ply.Position.z);

                            response = $"Every player's y position has been nudge by {val}";
                            return true;
                        case VectorAxis.Z:
                            foreach (Player ply in players)
                                ply.Position = new Vector3(ply.Position.x, ply.Position.y, ply.Position.z + val);

                            response = $"Every player's z position has been nudged by {val}";
                            return true;
                        default:
                            response = "\nUsage:\nposition (all / *) (set) (x position) (y position) (z position)\nposition (all / *) (get)\nposition (all / *) (add) (x, y, or z) (value)";
                            return false;
                    }
                    
                default:
                    response = "\nUsage:\nposition (all / *) (set) (x position) (y position) (z position)\nposition (all / *) (get)\nposition (all / *) (add) (x, y, or z) (value)";
                    return false;
            }
        }
    }
}
