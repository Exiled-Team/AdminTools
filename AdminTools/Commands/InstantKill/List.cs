using System;
using System.Linq;
using System.Text;
using CommandSystem;
using Exiled.API.Features.Pools;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.InstantKill
{
    public class List : ICommand
    {
        public string Command { get; } = "list";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Get a list of every player with InstantKill enabled.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.instakill"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            StringBuilder playerLister = StringBuilderPool.Pool.Get();

            playerLister.Append(Main.InstantKill.Any() ? "Players with InstantKill enabled:\n" : "No players currently online have instant killing on");
            playerLister.Append(Main.InstantKill.LogPlayers());

            response = StringBuilderPool.Pool.ToStringReturn(playerLister);
            return true;
        }
    }
}
