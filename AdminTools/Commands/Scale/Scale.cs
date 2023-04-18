﻿using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using AdminTools.Extensions;

namespace AdminTools.Commands.Scale
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Scale : ICommand
    {
        public string Command => "scale";

        public string[] Aliases => null;

        public string Description => "Scales all users or a user by a specified value";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.size"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\nscale ((player id / name) or (all / *)) (value)" +
                    "\nscale reset";
                return false;
            }

            switch (arguments.At(0))
            {
                case "reset":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: scale reset";
                        return false;
                    }
                    foreach (var plyr in Player.List)
                        plyr.SetPlayerScale(1);

                    response = $"Everyone's scale has been reset";
                    return true;
                case "*":
                case "all":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: scale (all / *) (value)";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(1), out var value))
                    {
                        response = $"Invalid value for scale: {arguments.At(1)}";
                        return false;
                    }

                    foreach (var ply in Player.List)
                        ply.SetPlayerScale(value);

                    response = $"Everyone's scale has been set to {value}";
                    return true;
                default:
                    if (arguments.Count != 2)
                    {
                        response = "Usage: scale (player id / name) (value)";
                        return false;
                    }

                    var pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return true;
                    }

                    if (!float.TryParse(arguments.At(1), out var val))
                    {
                        response = $"Invalid value for scale: {arguments.At(1)}";
                        return false;
                    }

                    pl.SetPlayerScale(val);
                    response = $"Player {pl.Nickname}'s scale has been set to {val}";
                    return true;
            }
        }
    }
}
