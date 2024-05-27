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
    internal class User : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
        {
            if (arguments.Count < 3)
            {
                response = "Usage: hbc user (player id / name) (time) (message)";
                return false;
            }

            Player ply = Player.Get(arguments.At(0));
            if (ply == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (!ushort.TryParse(arguments.At(1), out ushort time) && time <= 0)
            {
                response = $"Invalid value for duration: {arguments.At(1)}";
                return false;
            }

            ply.ShowHint(Extensions.FormatArguments(arguments, 2), time);
            response = $"Hint sent to {ply.Nickname}";
            return true;
        }

        public string Command { get; } = "user";
        public string[] Aliases { get; }
        public string Description { get; } = "Sends a broadcast to a specific user";
    }
}
