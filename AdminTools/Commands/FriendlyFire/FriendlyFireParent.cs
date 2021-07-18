using CommandSystem;
using System;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.FriendlyFire
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class FriendlyFireParent : ParentCommand
    {
        public FriendlyFireParent() => LoadGeneratedCommands();

        public override string Command { get; } = "friendlyfire";

        public override string[] Aliases { get; } = new string[] { "ff" };

        public override string Description { get; } = "Enable and Disable Friendly Fire";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Enable());
            RegisterCommand(new Disable());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.friendlyfire"))
            {
                response = "You do not have permission to use this command. (friendlyfirebetterperms.use)";
                return false;
            }
            else
            {
                response = "Usage: friendlyfire (enable/disable)";
                return false;
            }
        }
    }
}