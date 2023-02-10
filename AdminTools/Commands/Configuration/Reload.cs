using CommandSystem;
using GameCore;
using System;


namespace AdminTools.Commands.Configuration
{
    public class Reload : ICommand
    {
        public string Command { get; } = "reload";

        public string[] Aliases { get; } = new string[] { "rld" };

        public string Description { get; } = "Reloads all permissions and configs";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 0)
            {
                response = "Usage: cfig reload";
                return false;
            }

            
            ConfigFile.ReloadGameConfigs();
            response = "Configuration files reloaded!";
            return true;
        }
    }
}
