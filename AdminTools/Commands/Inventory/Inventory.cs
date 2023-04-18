namespace AdminTools.Commands.Inventory
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Inventory : ParentCommand
    {
        public Inventory()
        {
            LoadGeneratedCommands();
        }

        public override string Command => "inventory";

        public override string[] Aliases { get; } = { "inv" };

        public override string Description => "Manages player inventories";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Drop());
            RegisterCommand(new See());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender,
            out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.inv"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            response = "Invalid subcommand. Available ones: drop, see";
            return false;
        }
    }
}