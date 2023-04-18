using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using System.Linq;
using UnityEngine;

namespace AdminTools.Commands.RandomTeleport
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class RandomTeleport : ParentCommand
    {
        public RandomTeleport() => LoadGeneratedCommands();

        public override string Command => "randomtp";

        public override string[] Aliases => null;

        public override string Description => "Randomly teleports a user or all users to a random room in the facility";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "randomtp", PlayerPermissions.PlayersManagement, "AdminTools", false))
            {
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
                    foreach (var ply in Player.List)
                    {
                        var randRoom = Room.List.ElementAt(Plugin.NumGen.Next(0, Room.List.Count()));
                        ply.Position = randRoom.Position + Vector3.up;
                    }

                    response = $"Everyone was teleported to a random room in the facility";
                    return true;
                default:
                    var pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    var rand = Room.List.ElementAt(Plugin.NumGen.Next(0, Room.List.Count()));
                    pl.Position = rand.Position + Vector3.up;

                    response = $"Player {pl.Nickname} was teleported to {rand.Name}";
                    return true;
            }
        }
    }
}
