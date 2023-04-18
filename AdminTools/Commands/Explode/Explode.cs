namespace AdminTools.Commands.Explode
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Permissions.Extensions;
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Explode : ICommand
    {
        public string Command => "expl";

        public string[] Aliases { get; } = { "boom" };

        public string Description => "Explodes a specified user or everyone instantly";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
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
                        if (ply.Role == RoleTypeId.Spectator || ply.Role == RoleTypeId.None)
                            continue;

                        ply.Kill("Exploded by admin.");
                        ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                        grenade.FuseTime = 0.5f;
                        grenade.SpawnActive(ply.Position, ply);
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

                    if (pl.Role == RoleTypeId.Spectator || pl.Role == RoleTypeId.None)
                    {
                        response = $"Player \"{pl.Nickname}\" is not a valid class to explode";
                        return false;
                    }

                    pl.Kill("Exploded by admin.");
                    ExplosiveGrenade gr = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                    gr.FuseTime = 0.5f;
                    gr.SpawnActive(pl.Position, pl);

                    response = $"Player \"{pl.Nickname}\" game ended (exploded)";
                    return true;
            }
        }
    }
}