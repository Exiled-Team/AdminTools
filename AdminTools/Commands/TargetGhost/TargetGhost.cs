using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Linq;

namespace AdminTools.Commands.TargetGhost
{
    using Exiled.API.Features.Roles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class TargetGhost : ParentCommand
    {
        public const string HelpStr = "Usage: targetghost (player id / name) (player id / name) ...";

        public TargetGhost() => LoadGeneratedCommands();

        public override string Command { get; } = "targetghost";

        public override string[] Aliases { get; } = new string[] { "tg" };

        public override string Description { get; } = "Sets a user to be invisible to another user";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.targetghost"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = HelpStr;
                return false;
            }

            if (!GetPlayer(arguments.At(0), out var sourcePlayer))
            {
                response = $"Invalid source player: {arguments.At(0)}";
                return false;
            }

            foreach (var arg in arguments.Skip(1))
            {
                if (!GetPlayer(arg, out var victim))
                    continue;

                if (sourcePlayer.Role.Is(out FpcRole role))
                {
                    if (!role.IsInvisibleFor.Add(victim))
                        role.IsInvisibleFor.Remove(victim);
                }
            }
            response = $"Done modifying who can see {sourcePlayer.Nickname}";
            return true;
        }

        private bool GetPlayer(string str, out Player player)
        {
            player = Player.Get(str);
            return player != null;
        }
    }
}