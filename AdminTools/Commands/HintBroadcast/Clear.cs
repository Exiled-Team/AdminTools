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
    internal class Clear : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: hbc clear (user id / username)";
                return false;
            }

            Player ply = Player.Get(arguments.At(0));
            if (ply == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }
            ply.ShowHint(" ");
            response = $"Cleared hints of {ply.Nickname}";
            return true;
        }

        public string Command { get; } = "clear";
        public string[] Aliases { get; }
        public string Description { get; } = "Clears a user's hints";
    }
}
