using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Kill
{
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Kill : ParentCommand
    {
        public Kill() => LoadGeneratedCommands();

        public override string Command { get; } = "atkill";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Kills everyone or a user instantly";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
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
                    foreach (var ply in Player.List)
                    {
                        if (ply.Role == RoleTypeId.Spectator || ply.Role == RoleTypeId.None)
                            continue;

                        ply.Kill("Killed by admin.");
                    }

                    response = "Everyone has been game ended (killed) now";
                    return true;
                default:
                    var pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }
                    else if (pl.Role == RoleTypeId.Spectator || pl.Role == RoleTypeId.None)
                    {
                        response = $"Player {pl.Nickname} is not a valid class to kill";
                        return false;
                    }

                    pl.Kill("Killed by admin.");
                    response = $"Player {pl.Nickname} has been game ended (killed) now";
                    return true;
            }
        }
    }
}
