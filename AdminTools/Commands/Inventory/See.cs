using System;
using System.Text;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using NorthwoodLib.Pools;

namespace AdminTools.Commands.Inventory
{
    public class See : ICommand
    {
        public string Command { get; } = "see";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Sees the inventory items a user has";

        public string[] Usage { get; } = { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
                return false;

            if (arguments.Count != 1)
            {
                response = "Usage: inventory see (player id / name)";
                return false;
            }

            Player ply = Player.Get(arguments.At(0));
            if (ply == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            StringBuilder invBuilder = StringBuilderPool.Shared.Rent();
            if (ply.Items.Count != 0)
            {
                invBuilder.Append("Player ");
                invBuilder.Append(ply.Nickname);
                invBuilder.AppendLine(" has the following items in their inventory:");
                foreach (Item item in ply.Items)
                {
                    invBuilder.Append("- ");
                    invBuilder.AppendLine(item.Type.ToString());
                }
            }
            else
            {
                invBuilder.Append("Player ");
                invBuilder.Append(ply.Nickname);
                invBuilder.Append(" does not have any items in their inventory");
            }
            string msg = invBuilder.ToString();
            StringBuilderPool.Shared.Return(invBuilder);
            response = msg;
            return true;
        }
    }
}
