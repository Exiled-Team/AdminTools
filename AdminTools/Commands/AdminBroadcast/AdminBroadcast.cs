using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using AdminTools.Extensions;

namespace AdminTools.Commands.AdminBroadcast
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class AdminBroadcast : ICommand
    {
        public string Command => "adminbroadcast";

        public string[] Aliases { get; } = { "abc" };

        public string Description => "Sends a message to all currently online staff on the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "broadcast", 
                    PlayerPermissions.Broadcasting, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: adminbroadcast (time) (message)";
                return false;
            }

            if (!ushort.TryParse(arguments.At(0), out var t))
            {
                response = $"Invalid value for broadcast time: {arguments.At(0)}";
                return false;
            }

            foreach (var pl in Player.List)
            {
                if (pl.ReferenceHub.serverRoles.RemoteAdmin)
                    pl.Broadcast(t, arguments.FormatArguments(1) + $" ~{((CommandSender)sender).Nickname}", global::Broadcast.BroadcastFlags.AdminChat);
            }

            response = $"Message sent to all currently online staff";
            return true;
        }
    }
}
