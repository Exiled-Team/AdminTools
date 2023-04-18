namespace AdminTools.Commands.Cleanup
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    public class Ragdolls : ICommand
    {
        public string Command => "ragdolls";

        public string[] Aliases => null;

        public string Description => "Cleans up ragdolls on the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.cleanup"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: cleanup ragdolls";
                return false;
            }

            foreach (Ragdoll doll in Ragdoll.List)
            {
                doll.Destroy();
            }

            response = "Ragdolls have been cleaned up now";
            return true;
        }
    }
}