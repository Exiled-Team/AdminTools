namespace AdminTools.Commands.Configuration
{
    using System;
    using CommandSystem;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Configuration : ParentCommand
    {
        public Configuration()
        {
            LoadGeneratedCommands();
        }

        public override string Command => "cfig";

        public override string[] Aliases => null;

        public override string Description => "Manages reloading permissions and configs";

        public sealed override void LoadGeneratedCommands()
        {
            RegisterCommand(new Reload());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender,
            out string response)
        {
            response = "Invalid subcommmand. Available ones: reload";
            return false;
        }
    }
}