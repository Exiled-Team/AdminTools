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
    public class TargetGhost : ICommand
    {
        public const string HelpStr = "Usage: targetghost (player id / name) (player id / name) ...";
        
        public string Command => "targetghost";

        public string[] Aliases { get; } = { "tg" };

        public string Description => "Sets a user to be invisible to another user";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
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

            if (!GetPlayer(arguments.At(0), out Player sourcePlayer))
            {
                response = $"Invalid source player: {arguments.At(0)}";
                return false;
            }

            foreach (string arg in arguments.Skip(1))
            {
                if (!GetPlayer(arg, out Player victim))
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

        private static bool GetPlayer(string str, out Player player)
        {
            player = Player.Get(str);
            return player != null;
        }
    }
}