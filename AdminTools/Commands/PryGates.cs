﻿using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PryGates : ICommand
    {
        public string Command { get; } = "prygate";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Gives the ability to pry gates to players, clear the ability from players, and shows who has the ability";

        public void LoadGeneratedCommands() { }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.prygate"))
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

                    Main.PryGateHubs.Clear();
                    response = "The ability to pry gates is cleared from all players now";
                    return true;
                case "list":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: prygates list";
                        return false;
                    }

                    StringBuilder playerLister = StringBuilderPool.Shared.Rent(Main.PryGateHubs.Count != 0 ? "Players with the ability to pry gates:\n" : "No players currently online have the ability to pry gates");
                    if (Main.PryGateHubs.Count > 0)
                    {
                        foreach (Player ply in Main.PryGateHubs)
                            playerLister.Append(ply.Nickname + ", ");

                        int length = playerLister.ToString().Length;
                        response = playerLister.ToString().Substring(0, length - 2);
                        StringBuilderPool.Shared.Return(playerLister);
                        return true;
                    }
                    else
                    {
                        response = playerLister.ToString();
                        StringBuilderPool.Shared.Return(playerLister);
                        return true;
                    }
                case "remove":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: prygate remove (player id / name)";
                        return false;
                    }

                    players = Player.GetProcessedData(arguments, 1);

                    if (players.Count() is 0)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }
                    response = string.Empty;
                    foreach (Player ply in players)
                    {
                        if (Main.PryGateHubs.Remove(ply))
                        {
                            response += $"Player \"{ply.Nickname}\" cannot pry gates open now";
                            continue;
                        }
                        response += $"Player {ply.Nickname} does not have the ability to pry gates open";
                    }
                    return true;
                default:

                    players = Player.GetProcessedData(arguments);

                    if (arguments.Count != 1)
                    {
                        response = "Usage: prygates (all / *)";
                        return false;
                    }

                    foreach (Player ply in players)
                        Main.PryGateHubs.Add(ply);

                    response = "The ability to pry gates open is on for all players now";
                    return true;
            }
        }
    }
}