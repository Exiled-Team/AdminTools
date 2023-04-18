namespace AdminTools.Commands.Jail
{
    using System;
    using System.Linq;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using MEC;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Jail : ICommand
    {
        public string Command => "jail";

        public string[] Aliases => null;

        public string Description => "Jails or unjails a user";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.jail"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: jail (player id / name)";
                return false;
            }

            Player ply = Player.Get(arguments.At(0));
            if (ply == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (API.Jail.JailedPlayers.Any(j => j.UserId == ply.UserId))
            {
                try
                {
                    Timing.RunCoroutine(API.Jail.UnjailPlayer(ply));
                    response = $"Player {ply.Nickname} has been unjailed now";
                }
                catch (Exception e)
                {
                    Log.Error($"{e}");
                    response = "Command failed. Check server log.";
                    return false;
                }
            }
            else
            {
                Timing.RunCoroutine(API.Jail.JailPlayer(ply));
                response = $"Player {ply.Nickname} has been jailed now";
            }

            return true;
        }
    }
}