using CommandSystem;
using NorthwoodLib.Pools;
using System;
using AdminTools.API.Enums;

namespace AdminTools.Commands.Enums
{
    using Exiled.API.Enums;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Enums : ParentCommand
    {
        public Enums() => LoadGeneratedCommands();

        public override string Command { get; } = "enums";

        public override string[] Aliases { get; } = new string[] { "enum" };

        public override string Description { get; } = "Lists all enums AdminTools uses";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var listBuilder = StringBuilderPool.Shared.Rent();
            listBuilder.Append("Here are the following enums you can use in commands:");
            listBuilder.AppendLine();
            listBuilder.Append("ItemType: ");
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
            {
                listBuilder.Append(type.ToString());
                listBuilder.Append(" ");
            }
            listBuilder.AppendLine();
            listBuilder.Append("ProjectileType: ");
            foreach (ProjectileType gt in Enum.GetValues(typeof(ProjectileType)))
            {
                listBuilder.Append(gt.ToString());
                listBuilder.Append(" ");
            }
            listBuilder.AppendLine();
            listBuilder.Append("VectorAxis: ");
            foreach (VectorAxis va in Enum.GetValues(typeof(VectorAxis)))
            {
                listBuilder.Append(va.ToString());
                listBuilder.Append(" ");
            }
            listBuilder.AppendLine();
            listBuilder.Append("PositionModifier: ");
            foreach (PositionModifier pm in Enum.GetValues(typeof(PositionModifier)))
            {
                listBuilder.Append(pm.ToString());
                listBuilder.Append(" ");
            }
            var message = listBuilder.ToString();
            StringBuilderPool.Shared.Return(listBuilder);
            response = message;
            return true;
        }
    }
}
