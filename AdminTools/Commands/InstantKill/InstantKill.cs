using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using System;
using System.Linq;
using AdminTools.Extensions;

namespace AdminTools.Commands.InstantKill
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class InstantKill : ICommand
    {
        public const string InstantKillSessionVariableName = "AT-InstantKill";
        
        public string Command => "instakill";

        public string[] Aliases { get; } = { "ik" };

        public string Description => "Manage instant kill properties for users";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.ik"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\ninstakill ((player id / name) or (all / *))" +
                    "\ninstakill clear" +
                    "\ninstakill list" +
                    "\ninstakill remove (player id / name)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "clear":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: instakill clear";
                        return false;
                    }

                    foreach (var ply in Player.List)
                    {
                        if (!ply.HasSessionVariable(InstantKillSessionVariableName)) continue;

                        ply.RemoveSessionVariable(InstantKillSessionVariableName);
                    }

                    response = "Instant killing has been removed from everyone";
                    return true;
                case "list":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: instakill clear";
                        return false;
                    }

                    var instantKillingHubs = Player.Get(p => p.HasSessionVariable(InstantKillSessionVariableName)).ToList();
                    
                    var playerLister = StringBuilderPool.Shared.Rent(instantKillingHubs.Count == 0?
                        "No players currently online have instant killing on" : "Players with instant killing on:\n");
                    
                    if (instantKillingHubs.Count == 0)
                    {
                        response = playerLister.ToString();
                        return true;
                    }

                    foreach (var ply in instantKillingHubs)
                    {
                        playerLister.Append(ply.Nickname);
                        playerLister.Append(", ");
                    }

                    var msg = playerLister.ToString().Substring(0, playerLister.ToString().Length - 2);
                    StringBuilderPool.Shared.Return(playerLister);
                    
                    response = msg;
                    return true;
                case "remove":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: instakill remove (player id / name)";
                        return false;
                    }

                    var pl = Player.Get(arguments.At(1));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (pl.HasSessionVariable(InstantKillSessionVariableName))
                    {
                        pl.RemoveSessionVariable(InstantKillSessionVariableName);
                        response = $"Instant killing is off for {pl.Nickname} now";
                    }
                    else response = $"Player {pl.Nickname} does not have the ability to instantly kill others";
                    
                    return true;
                case "*":
                case "all":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: instakill all / *";
                        return false;
                    }

                    foreach (var ply in Player.List)
                    {
                        if (!ply.HasSessionVariable(InstantKillSessionVariableName))
                            ply.AddBooleanSessionVariable(InstantKillSessionVariableName);
                    }

                    response = "Everyone on the server can instantly kill other users now";
                    return true;
                default:
                    if (arguments.Count != 1)
                    {
                        response = "Usage: instakill (player id / name)";
                        return false;
                    }

                    var plyr = Player.Get(arguments.At(0));
                    if (plyr == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!plyr.HasSessionVariable(InstantKillSessionVariableName))
                    {
                        plyr.AddBooleanSessionVariable(InstantKillSessionVariableName);
                        response = $"Instant killing is on for {plyr.Nickname}";
                    }
                    else
                    {
                        plyr.RemoveSessionVariable(InstantKillSessionVariableName);
                        response = $"Instant killing is off for {plyr.Nickname}";
                    }
                    
                    return true;
            }
        }
    }
}
