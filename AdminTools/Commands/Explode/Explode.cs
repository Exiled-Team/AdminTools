using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Grenades;
using System;

namespace AdminTools.Commands.Explode
{
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
            EventHandlers.LogCommandUsed((CommandSender)sender, EventHandlers.FormatArguments(arguments, 0));
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

                    foreach (Player Ply in Player.List)
                    {
                        if (Ply.Role == RoleType.Spectator || Ply.Role == RoleType.None)
                            continue;

                        Ply.Hurt(-1, null, "ADMIN");
                        EventHandlers.SpawnGrenadeOnPlayer(Ply, GrenadeType.Frag, 0.1f);
                    }
                    response = "Everyone exploded, Hubert cannot believe you have done this";
                    return true;
                default:
                    if (arguments.Count != 1)
                    {
                        response = "Usage: expl (player id / name)";
                        return false;
                    }

                    Player Pl = Player.Get(arguments.At(0));
                    if (Pl == null)
                    {
                        response = $"Invalid target to explode: {arguments.At(0)}";
                        return false;
                    }

                    if (Pl.Role == RoleType.Spectator || Pl.Role == RoleType.None)
                    {
                        response = $"Player \"{Pl.Nickname}\" is not a valid class to explode";
                        return false;
                    }

                    Pl.Hurt(-1, null, "ADMIN");
                    EventHandlers.SpawnGrenadeOnPlayer(Pl, GrenadeType.Frag, 0.1f);
                    response = $"Player \"{Pl.Nickname}\" game ended (exploded)";
                    return true;
            }
        }
    }
}
