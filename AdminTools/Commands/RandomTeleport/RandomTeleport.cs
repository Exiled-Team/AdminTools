using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using System.Linq;
using UnityEngine;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.RandomTeleport
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class RandomTeleport : ParentCommand
    {
        public RandomTeleport() => LoadGeneratedCommands();

        public override string Command { get; } = "randomtp";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Randomly teleports a user or all users to a random room in the facility";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.tp")) { 

                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: randomtp ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    foreach (Player ply in Player.List)
                    {
                        Room randRoom = Room.List.ElementAt(Plugin.NumGen.Next(0, Room.List.Count()));
                        ply.Position = randRoom.Position + Vector3.up;
                    }

                    response = $"Everyone was teleported to a random room in the facility";
                    return true;
                default:
                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    Room rand = Room.List.ElementAt(Plugin.NumGen.Next(0, Room.List.Count()));
                    pl.Position = rand.Position + Vector3.up;

                    response = $"Player {pl.Nickname} was teleported to {rand.Name}";
                    return true;
            }
        }
    }
}
