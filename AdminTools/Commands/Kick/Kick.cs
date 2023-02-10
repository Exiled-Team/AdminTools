using CommandSystem;
using Exiled.Permissions.Extensions;
using System;
using UnityEngine;

namespace AdminTools.Commands.Kick
{
    using Exiled.API.Features;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Kick : ParentCommand
    {
        public Kick() => LoadGeneratedCommands();

        public override string Command { get; } = "kick";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Kicks a player from the game with a custom reason";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.kick"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: kick (player id / name) (reason)";
                return false;
            }

            Player ply = Player.Get(arguments.At(0));
            if (ply == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }


            ply.Kick(EventHandlers.FormatArguments(arguments, 1));
            response = $"Player {ply.Nickname} has been kicked for \"{EventHandlers.FormatArguments(arguments, 1)}\"";
            return true;
        }
    }
}
