using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;

namespace AdminTools.Commands.AdminBroadcast
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class AdminBroadcast : ParentCommand
    {
        public AdminBroadcast() => LoadGeneratedCommands();

        public override string Command { get; } = "adminbroadcast";

        public override string[] Aliases { get; } = new string[] { "abc" };

        public override string Description { get; } = "Sends a message to all currently online staff on the server";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            EventHandlers.LogCommandUsed((CommandSender)sender, EventHandlers.FormatArguments(arguments, 0));
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "broadcast", PlayerPermissions.Broadcasting, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: adminbroadcast (time) (message)";
                return false;
            }

            if (!ushort.TryParse(arguments.At(0), out ushort t))
            {
                response = $"Invalid value for broadcast time: {arguments.At(0)}";
                return false;
            }

            foreach (Player Pl in Player.List)
            {
                if (Pl.Sender.CheckPermission(PlayerPermissions.AdminChat))
                    Pl.Broadcast(t, EventHandlers.FormatArguments(arguments, 1) + $" ~{((CommandSender)sender).Nickname}", Broadcast.BroadcastFlags.AdminChat);
            }

            response = $"Message sent to all currently online staff";
            return true;
        }
    }
}
