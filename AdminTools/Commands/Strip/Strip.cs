namespace AdminTools.Commands.Strip
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using RemoteAdmin;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Strip : ICommand
    {
        public string Command => "atstrip";

        public string[] Aliases { get; } = { "stp" };

        public string Description => "Clears a user or users inventories instantly";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions((CommandSender)sender, "strip", PlayerPermissions.PlayersManagement,
                    "AdminTools", false))
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
                    {
                        ply.ClearInventory();
                    }

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