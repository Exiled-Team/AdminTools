using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Kill
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Kill : ParentCommand
    {
        public Kill() => LoadGeneratedCommands();

        public override string Command { get; } = "kill";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Kills everyone or a user instantly";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            EventHandlers.LogCommandUsed((CommandSender)sender, EventHandlers.FormatArguments(arguments, 0));
            if (!((CommandSender)sender).CheckPermission("at.kill"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: kill ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    foreach (Player Ply in Player.List)
                    {
                        if (Ply.Role == RoleType.Spectator || Ply.Role == RoleType.None)
                            continue;

                        Ply.Hurt(-1, null, "ADMIN");
                    }

                    response = "Everyone has been game ended (killed) now";
                    return true;
                default:
                    Player Pl = Player.Get(arguments.At(0));
                    if (Pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }
                    else if (Pl.Role == RoleType.Spectator || Pl.Role == RoleType.None)
                    {
                        response = $"Player {Pl.Nickname} is not a valid class to kill";
                        return false;
                    }

                    Pl.Hurt(-1, null, "ADMIN");
                    response = $"Player {Pl.Nickname} has been game ended (killed) now";
                    return true;
            }
        }
    }
}
