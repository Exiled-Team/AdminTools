using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Explode
{
    using Exiled.API.Features.Items;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Explode : ParentCommand
    {
        public Explode() => LoadGeneratedCommands();

        public override string Command { get; } = "expl";

        public override string[] Aliases { get; } = new string[] { "boom" };

        public override string Description { get; } = "Explodes a specified user or everyone instantly";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.explode"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: expl ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: expl (all / *)";
                        return false;
                    }

                    foreach (Player ply in Player.List)
                    {
                        if (ply.Role == RoleType.Spectator || ply.Role == RoleType.None)
                            continue;

                        ply.Kill("Exploded by admin.");
                        EventHandlers.Explode(ply.Position, ply.ReferenceHub);
                    }
                    response = "Everyone exploded, Hubert cannot believe you have done this";
                    return true;
                default:
                    if (arguments.Count != 1)
                    {
                        response = "Usage: expl (player id / name)";
                        return false;
                    }

                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Invalid target to explode: {arguments.At(0)}";
                        return false;
                    }

                    if (pl.Role == RoleType.Spectator || pl.Role == RoleType.None)
                    {
                        response = $"Player \"{pl.Nickname}\" is not a valid class to explode";
                        return false;
                    }

                    pl.Kill("Exploded by admin.");
                    EventHandlers.Explode(pl.Position, pl.ReferenceHub);
                    response = $"Player \"{pl.Nickname}\" game ended (exploded)";
                    return true;
            }
        }
    }
}
