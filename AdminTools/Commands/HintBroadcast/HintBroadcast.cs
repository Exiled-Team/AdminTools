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
            RegisterCommand(new Clear());
            RegisterCommand(new Groups());
            RegisterCommand(new User());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response) // TODO: Make it ParentCommand
        {
            response = "Not a valid subcommand. Available subcommands: user, groups, clear";
            return false;
        }
    }
}
