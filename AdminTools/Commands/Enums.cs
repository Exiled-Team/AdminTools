using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Enums : ICommand, IUsageProvider
    {
        private IEnumerable<Type> types;
        public string Command { get; } = "enums";

        public string[] Aliases { get; } = { "enum" };

        public string Description { get; } = "Lists all enums";

        public string[] Usage { get; } = { "EnumName", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count > 1)
            {
                response = "Give an Enum name (ProjectileType, RoleTypeId, ItemType, VectorAxis, PositionModifier)";
                return false;
            }

            types ??= AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(y => y.IsEnum);

            Type enumType = types.FirstOrDefault(t => t.Name.Contains(arguments.At(0)));
            if (enumType == null)
            {
                response = "Invalid Enum name";
                return false;
            }
            response = $"<b>{enumType.Name}<\b>\n - ";
            response += string.Join("\n - ", Enum.GetNames(enumType));
            return true;
        }
    }
}
