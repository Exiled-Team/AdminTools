using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using UnityEngine;

namespace AdminTools.Commands.DropItem
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class DropItem : ParentCommand
    {
        public DropItem() => LoadGeneratedCommands();

        public override string Command { get; } = "dropitem";

        public override string[] Aliases { get; } = new string[] { "drop", "dropi" };

        public override string Description { get; } = "Drops a specified amount of a specified item on either all users or a user";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            EventHandlers.LogCommandUsed((CommandSender)sender, EventHandlers.FormatArguments(arguments, 0));
            if (!((CommandSender)sender).CheckPermission("at.items"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 3)
            {
                response = "Usage: dropitem ((player id/ name) or (all / *)) (ItemType) (amount (200 max for one user, 15 max for all users))";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    if (arguments.Count != 3)
                    {
                        response = "Usage: dropitem (all / *) (ItemType) (amount (15 max))";
                        return false;
                    }

                    if (!Enum.TryParse(arguments.At(1), true, out ItemType Item))
                    {
                        response = $"Invalid value for item type: {arguments.At(1)}";
                        return false;
                    }

                    if (!uint.TryParse(arguments.At(2), out uint Amount) || Amount > 15)
                    {
                        response = $"Invalid amount of item to drop: {arguments.At(2)} {(Amount > 15 ? "(\"Try a lower number that won't crash my servers, ty.\" - Galaxy119)" : "")}";
                        return false;
                    }

                    foreach (Player Ply in Player.List)
                        for (int i = 0; i < Amount; i++)
                            EventHandlers.SpawnItem(Item, Ply.Position, Vector3.zero);

                    response = $"{Amount} of {Item.ToString()} was spawned on everyone (\"Hehexd\" - Galaxy119)";
                    return true;
                default:
                    if (arguments.Count != 3)
                    {
                        response = "Usage: dropitem (player id / name) (ItemType) (amount (200 max))";
                        return false;
                    }

                    Player Pl = Player.Get(arguments.At(0));
                    if (Pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!Enum.TryParse(arguments.At(1), true, out ItemType It))
                    {
                        response = $"Invalid value for item type: {arguments.At(1)}";
                        return false;
                    }

                    if (!uint.TryParse(arguments.At(2), out uint Am) || Am > 200)
                    {
                        response = $"Invalid amount of item to drop: {arguments.At(2)}";
                        return false;
                    }

                    for (int i = 0; i < Am; i++)
                        EventHandlers.SpawnItem(It, Pl.Position, Vector3.zero);
                    response = $"{Am} of {It.ToString()} was spawned on {Pl.Nickname} (\"Hehexd\" - Galaxy119)";
                    return true;
            }
        }
    }
}
