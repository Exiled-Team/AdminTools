using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.InstantKill
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class InstantKill : ParentCommand, IUsageProvider
    {
        public InstantKill() => LoadGeneratedCommands();
        public override string Command { get; } = "instantkill";

        public override string[] Aliases { get; } = new string[] { "ik", "instakill" };

        public override string Description { get; } = "Manage instant kill properties for users";

        public string[] Usage { get; } = new string[] { "Add or Remove or List", };

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Add());
            RegisterCommand(new Remove());
            RegisterCommand(new List());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.instakill"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            response = "Usage:" +
                "\nInstantKill add ((player id / name) or (all / *))" +
                "\nInstantKill list" +
                "\nInstantKill remove (player id / name) or (all / *))";
            return false;

        }
    }
}
