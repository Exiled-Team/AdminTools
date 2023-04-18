namespace AdminTools.Commands.PryGates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using Extensions;
    using NorthwoodLib.Pools;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PryGates : ICommand
    {
        public const string PryGatesSessionVariableName = "AT-PryGates";

        public string Command => "prygate";

        public string[] Aliases => null;

        public string Description =>
            "Gives the ability to pry gates to players, clear the ability from players, and shows who has the ability";

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

            switch (arguments.At(0))
            {
                case "clear":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: prygates clear";
                        return false;
                    }

                    foreach (Player player in Player.List)
                    {
                        if (player.HasSessionVariable(PryGatesSessionVariableName))
                            player.RemoveSessionVariable(PryGatesSessionVariableName);
                    }

                    response = "The ability to pry gates is cleared from all players now";
                    return true;
                case "list":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: prygates list";
                        return false;
                    }

                    List<Player> pryGateHubs =
                        Player.Get(p => p.HasSessionVariable(PryGatesSessionVariableName)).ToList();

                    StringBuilder playerLister = StringBuilderPool.Shared.Rent(pryGateHubs.Count != 0
                        ? "Players with the ability to pry gates:\n"
                        : "No players currently online have the ability to pry gates");
                    if (pryGateHubs.Count > 0)
                    {
                        foreach (Player ply in pryGateHubs)
                        {
                            playerLister.Append(ply.Nickname + ", ");
                        }

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

                    Player plyr = Player.Get(arguments.At(1));
                    if (plyr == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (plyr.HasSessionVariable(PryGatesSessionVariableName))
                    {
                        plyr.RemoveSessionVariable(PryGatesSessionVariableName);
                        response = $"Player \"{plyr.Nickname}\" cannot pry gates open now";
                    }
                    else
                    {
                        response = $"Player {plyr.Nickname} does not have the ability to pry gates open";
                    }

                    return true;
                case "*":
                case "all":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: prygates (all / *)";
                        return false;
                    }

                    foreach (Player ply in Player.List)
                    {
                        if (!ply.HasSessionVariable(PryGatesSessionVariableName))
                            ply.AddBooleanSessionVariable(PryGatesSessionVariableName);
                    }

                    response = "The ability to pry gates open is on for all players now";
                    return true;
                default:
                    if (arguments.Count != 1)
                    {
                        response = "Usage: prygate (player id / name)";
                        return false;
                    }

                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player \"{arguments.At(0)}\" not found";
                        return false;
                    }


                    if (!pl.HasSessionVariable(PryGatesSessionVariableName))
                    {
                        pl.AddBooleanSessionVariable(PryGatesSessionVariableName);
                        response = $"Player \"{pl.Nickname}\" can now pry gates open";
                        return true;
                    }

                    pl.RemoveSessionVariable(PryGatesSessionVariableName);

                    response = $"Player \"{pl.Nickname}\" cannot pry gates open now";
                    return true;
            }
        }
    }
}