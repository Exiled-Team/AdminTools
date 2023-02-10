using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.Strip
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Strip : ParentCommand
    {
        public Strip() => LoadGeneratedCommands();

        public override string Command { get; } = "atstrip";

        public override string[] Aliases { get; } = new string[] { "stp" };

        public override string Description { get; } = "Clears a user or users inventories instantly";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.strip"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: strip ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    foreach (Player ply in Player.List)
                        ply.ClearInventory();

                    response = "Everyone's inventories have been cleared now";
                    return true;
                default:
                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    pl.ClearInventory();
                    response = $"Player {pl.Nickname}'s inventory have been cleared now";
                    return true;
            }
        }
    }
}
