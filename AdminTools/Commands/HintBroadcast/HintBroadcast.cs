using CommandSystem;
using Exiled.API.Features;
using NorthwoodLib.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdminTools.Commands.HintBroadcast
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class HintBroadcast : ParentCommand, IUsageProvider
    {
        public override string Command { get; } = "hintbroadcast";

        public override string[] Aliases { get; } = new string[] { "hint" , "hbc" };

        public override string Description { get; } = "Broadcasts a message to either a user, a group, a role, all staff, or everyone";

        public string[] Usage { get; } = new string[] { "Uhm" };
        public override void LoadGeneratedCommands()
        {
            // RegisterCommand(new Add());
            // RegisterCommand(new Clear());
            RegisterCommand(new Group());
            RegisterCommand(new Groups());
            RegisterCommand(new User());
            RegisterCommand(new Users());
            RegisterCommand(new Random());
            RegisterCommand(new Staff());
            RegisterCommand(new Clearall());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response) // TODO: Make it ParentCommand
        {
            if (arguments.Count < 2)
            {
                response = "Usage: hbc (time) (message) Available subcommands: hint, user, group, groups, random/someone, staff/admin, clearall";
                return false;
            }

            if (!ushort.TryParse(arguments.At(0), out ushort tm))
            {
                response = $"Invalid value for hint broadcast time: {arguments.At(0)}";
                return false;
            }

            foreach (Player py in Player.List)
                py.ShowHint(Extensions.FormatArguments(arguments, 1), tm);

            response = "Sent a broadcast to everyone. Available subcommands: hint, user, group, groups, random/someone, staff/admin, clearall";
            return false;
        }
    }
}
