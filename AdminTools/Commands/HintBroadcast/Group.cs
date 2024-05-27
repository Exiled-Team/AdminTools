using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.API.Features;

namespace AdminTools.Commands.HintBroadcast
{
    internal class Group : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
        {
            if (arguments.Count < 3)
            {
                response = "Usage: hbc group (group) (time) (message)";
                return false;
            }

            UserGroup broadcastGroup = ServerStatic.PermissionsHandler.GetGroup(arguments.At(0));
            if (broadcastGroup == null)
            {
                response = $"Invalid group: {arguments.At(0)}";
                return false;
            }

            if (!ushort.TryParse(arguments.At(1), out ushort tim) && tim <= 0)
            {
                response = $"Invalid value for duration: {arguments.At(1)}";
                return false;
            }

            foreach (Player player in Player.List)
            {
                if (player.Group.BadgeText.Equals(broadcastGroup.BadgeText))
                    player.ShowHint(Extensions.FormatArguments(arguments, 2), tim);
            }

            response = $"Hint sent to all members of \"{arguments.At(0)}\"";
            return true;
        }

        public string Command { get; } = "group";
        public string[] Aliases { get; }
        public string Description { get; } = "Sends a broadcast to every member of a group";
    }
}
