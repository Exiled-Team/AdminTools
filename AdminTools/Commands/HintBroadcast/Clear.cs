using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;

namespace AdminTools.Commands.HintBroadcast
{
    internal class Clear : ICommand
    {
        public string Command { get; } = "clear";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Clears a user's hints";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: hbc clear (user ids / usernames)";
                return false;
            }

            IEnumerable<Player> ply = Player.GetProcessedData(arguments);
            if (ply.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            foreach (Player player in ply)
            {
                player.ShowHint(string.Empty);
            }
            response = "Cleared hints of users";
            return true;
        }
    }
}
