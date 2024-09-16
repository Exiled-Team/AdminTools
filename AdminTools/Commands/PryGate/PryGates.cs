using System;
using System.Collections.Generic;
using System.Text;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;

namespace AdminTools.Commands.PryGate
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PryGates : ICommand, IUsageProvider
    {
        public string Command { get; } = "prygate";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Gives the ability to pry gates to players, clear the ability from players, and shows who has the ability";

        public string[] Usage { get; } = { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.prygate"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\nprygate ((player id / name) or (all / *))" +
                    "\nprygate clear" +
                    "\nprygate list" +
                    "\nprygate remove (player id / name)";
                return false;
            }

            IEnumerable<Player> players;

            switch (arguments.At(0))
            {
                case "clear":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: prygates clear";
                        return false;
                    }

                    Main.PryGate.Clear();
                    response = "The ability to pry gates is cleared from all players now";
                    return true;
                case "list":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: prygates list";
                        return false;
                    }

                    StringBuilder playerLister = StringBuilderPool.Shared.Rent(Main.PryGate.Count != 0 ? "Players with the ability to pry gates:\n" : "No players currently online have the ability to pry gates");
                    if (Main.PryGate.Count > 0)
                    {
                        foreach (Player ply in Main.PryGate)
                            playerLister.Append(ply.Nickname + ", ");

                        int length = playerLister.ToString().Length;
                        response = playerLister.ToString().Substring(0, length - 2);
                        StringBuilderPool.Shared.Return(playerLister);
                        return true;
                    }

                    response = playerLister.ToString();
                    StringBuilderPool.Shared.Return(playerLister);
                    return true;
                case "remove":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: prygate remove (player id / name)";
                        return false;
                    }

                    players = Player.GetProcessedData(arguments, 1);

                    if (players.IsEmpty())
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }
                    response = string.Empty;
                    foreach (Player ply in players)
                    {
                        if (Main.PryGate.Remove(ply))
                        {
                            response += $"Player \"{ply.Nickname}\" can no longer pry gates open";
                            continue;
                        }
                        response += $"Player {ply.Nickname} doesn't have the ability to pry gates open";
                    }
                    return true;
                default:
                    if (arguments.Count != 1)
                    {
                        response = "Usage: prygates (all / *)";
                        return false;
                    }
                    players = Player.GetProcessedData(arguments);

                    if (players.IsEmpty())
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    foreach (Player ply in players)
                        Main.PryGate.Add(ply);

                    response = "Every player can now pry gates open.";
                    return true;
            }
        }
    }
}
