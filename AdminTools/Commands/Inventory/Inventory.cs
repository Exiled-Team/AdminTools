using System;
using CommandSystem;

namespace AdminTools.Commands.Inventory
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Inventory : ParentCommand, IUsageProvider
    {
        public Inventory() => LoadGeneratedCommands();

        public override string Command { get; } = "inventory";

        public override string[] Aliases { get; } = { "inv" };

        public override string Description { get; } = "Manages player inventories";

        public string[] Usage { get; } = { "drop / see", };

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Drop());
            RegisterCommand(new See());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
                return false;

            response = "Invalid subcommand. Available ones: drop, see";
            return false;
        }
    }
}
