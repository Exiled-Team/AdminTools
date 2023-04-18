namespace AdminTools.Commands.Cleanup
{
    using System;
    using CommandSystem;
    using Exiled.API.Features.Pickups;
    using Exiled.Permissions.Extensions;

    public class Items : ICommand
    {
        public string Command => "items";

        public string[] Aliases => null;

        public string Description => "Cleans up items dropped on the ground from the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.cleanup"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: cleanup items";
                return false;
            }

            foreach (Pickup item in Pickup.List)
            {
                item.Destroy();
            }

            response = "Items have been cleaned up now";
            return true;
        }
    }
}