using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.DropItem
{
    using Exiled.API.Features.Pickups;

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

                    if (!Enum.TryParse(arguments.At(1), true, out ItemType item))
                    {
                        response = $"Invalid value for item type: {arguments.At(1)}";
                        return false;
                    }

                    if (!uint.TryParse(arguments.At(2), out var amount) || amount > 15)
                    {
                        response = $"Invalid amount of item to drop: {arguments.At(2)} {(amount > 15 ? "(\"Try a lower number that won't crash my servers, ty.\" - Galaxy119)" : "")}";
                        return false;
                    }

                    foreach (var ply in Player.List)
                        for (var i = 0; i < amount; i++)
                            Pickup.CreateAndSpawn(item, ply.Position, default, ply);

                    response = $"{amount} of {item.ToString()} was spawned on everyone (\"Hehexd\" - Galaxy119)";
                    return true;
                default:
                    if (arguments.Count != 3)
                    {
                        response = "Usage: dropitem (player id / name) (ItemType) (amount (200 max))";
                        return false;
                    }

                    var pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!Enum.TryParse(arguments.At(1), true, out ItemType it))
                    {
                        response = $"Invalid value for item type: {arguments.At(1)}";
                        return false;
                    }

                    if (!uint.TryParse(arguments.At(2), out var am) || am > 200)
                    {
                        response = $"Invalid amount of item to drop: {arguments.At(2)}";
                        return false;
                    }

                    for (var i = 0; i < am; i++)
                        Pickup.CreateAndSpawn(it, pl.Position, default, pl);
                    response = $"{am} of {it.ToString()} was spawned on {pl.Nickname} (\"Hehexd\" - Galaxy119)";
                    return true;
            }
        }
    }
}
