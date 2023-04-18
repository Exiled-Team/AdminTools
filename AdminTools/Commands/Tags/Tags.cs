namespace AdminTools.Commands.Tags
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Tags : ParentCommand
    {
        public Tags()
        {
            LoadGeneratedCommands();
        }

        public override string Command => "tags";

        public override string[] Aliases => null;

        public override string Description => "Hides staff tags in the server";

        public sealed override void LoadGeneratedCommands()
        {
            RegisterCommand(new Hide());
            RegisterCommand(new Show());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender,
            out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.tags"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            response = "Invalid subcommand. Available ones: hide, show";
            return false;
        }
    }
}