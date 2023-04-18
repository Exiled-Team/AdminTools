namespace AdminTools.Commands.Configuration
{
    using System;
    using CommandSystem;
    using GameCore;

    public class Reload : ICommand
    {
        public string Command => "reload";

        public string[] Aliases { get; } = { "rld" };

        public string Description => "Reloads all permissions and configs";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 0)
            {
                response = "Usage: cfig reload";
                return false;
            }

            ServerStatic.PermissionsHandler.RefreshPermissions();
            ConfigFile.ReloadGameConfigs();

            response = "Configuration files reloaded!";
            return true;
        }
    }
}