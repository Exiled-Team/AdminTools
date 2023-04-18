namespace AdminTools.Commands.Hp
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using RemoteAdmin;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Hp : ParentCommand
    {
        public Hp()
        {
            LoadGeneratedCommands();
        }

        public override string Command => "athp";

        public override string[] Aliases => null;

        public override string Description => "Sets a user or users HP to a specified value";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender,
            out string response)
        {
            if (!CommandProcessor.CheckPermissions((CommandSender)sender, "hp", PlayerPermissions.PlayersManagement,
                    "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 2)
            {
                response = "Usage: hp ((player id / name) or (all / *)) (value)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    if (!int.TryParse(arguments.At(1), out int value))
                    {
                        response = $"Invalid value for HP: {value}";
                        return false;
                    }

                    foreach (Player pl in Player.List)
                    {
                        if (value <= 0)
                            pl.Kill("Killed by admin.");
                        else
                            pl.Health = value;
                    }

                    response = $"Everyone's HP was set to {value}";
                    return true;
                default:
                    Player ply = Player.Get(arguments.At(0));
                    if (ply == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!int.TryParse(arguments.At(1), out int val))
                    {
                        response = $"Invalid value for HP: {val}";
                        return false;
                    }

                    if (val <= 0)
                        ply.Kill("Killed by admin.");
                    else
                        ply.Health = val;
                    response = $"Player {ply.Nickname}'s HP was set to {val}";
                    return true;
            }
        }
    }
}