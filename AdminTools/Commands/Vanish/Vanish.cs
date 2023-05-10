using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Linq;
using Exiled.API.Extensions;
using PlayerRoles;
using Utils.NonAllocLINQ;

namespace AdminTools.Commands.Vanish
{
    using CustomPlayerEffects;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Vanish : ParentCommand
    {
        public Vanish() => LoadGeneratedCommands();

        public override string Command { get; } = "vanish";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Sets a user to be vanished";
        
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.vanish"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage:\nvanish ((player id / name)";
                return false;
            }
            
            Player ply = Player.Get(arguments.At(0));
            if (ply == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }
            
            if (!ply.IsVanished())
            {
                var targets = Player.List.Where(pl => pl != ply);
                
                if (Plugin.Instance.Config.VanishedSeeEachOther)
                    targets = targets.Where(pl => !pl.IsVanished());
                
                ply.ChangeAppearance(RoleTypeId.Spectator, targets);
                response = $"Player {ply.Nickname} is now vanished";
            }
            else
            {
                Plugin.VanishedPlayers.ForEachKey(vanished => vanished.ChangeAppearance(RoleTypeId.Spectator, Enumerable.Repeat(ply, 1),0));

                ply.ChangeAppearance(Plugin.VanishedPlayers[ply], 0);
                response = $"Player {ply.Nickname} is no longer vanished";
            }
            return true;
        }
    }
}
