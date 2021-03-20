using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;

namespace AdminTools.Commands.Ahp
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ahp : ParentCommand
    {
        public Ahp() => LoadGeneratedCommands();

        public override string Command { get; } = "ahp";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Sets a user or users Artificial HP to a specified value";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            EventHandlers.LogCommandUsed((CommandSender)sender, EventHandlers.FormatArguments(arguments, 0));
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "ahp", PlayerPermissions.PlayersManagement, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 2)
            {
                response = "Usage: ahp ((player id / name) or (all / *)) (value)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    if (!int.TryParse(arguments.At(1), out int value) || value < 0)
                    {
                        response = $"Invalid value for AHP: {value}";
                        return false;
                    }

                    foreach (Player Ply in Player.List)
                        Ply.ArtificialHealth = value;

                    response = $"Everyone's AHP was set to {value}";
                    return true;
                default:
                    Player Pl = Player.Get(arguments.At(0));
                    if (Pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!int.TryParse(arguments.At(1), out int val) || val < 0)
                    {
                        response = $"Invalid value for AHP: {val}";
                        return false;
                    }

                    Pl.ArtificialHealth = val;
                    response = $"Player {Pl.Nickname}'s AHP was set to {val}";
                    return true;
            }
        }
    }
}
