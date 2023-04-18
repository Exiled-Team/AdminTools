using CommandSystem;
using Exiled.API.Features;
using NorthwoodLib.Pools;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using AdminTools.Extensions;

namespace AdminTools.Commands.HintBroadcast
{
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class HintBroadcast : ICommand
    {
        public string Command => "hbc";

        public string[] Aliases { get; } = { "broadcasthint" };

        public string Description => "Broadcasts a message to either a user, a group, a role, all staff, or everyone";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "hints", PlayerPermissions.Broadcasting, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\nhint (time) (message)" +
                    "\nhbc user (player id / name) (time) (message)" +
                    "\nhbc users (player id / name group (i.e.: 1,2,3 or hello,there,hehe)) (time) (message)" +
                    "\nhbc group (group name) (time) (message)" +
                    "\nhbc groups (list of groups (i.e.: owner,admin,moderator)) (time) (message)" +
                    "\nhbc role (RoleTypeId) (time) (message)" +
                    "\nhbc roles (RoleTypeId group (i.e.: ClassD,Scientist,NtfCadet)) (time) (message)" +
                    "\nhbc (random / someone) (time) (message)" +
                    "\nhbc (staff / admin) (time) (message)" +
                    "\nhbc clearall";
                return false;
            }

            var formatArguments3 = arguments.FormatArguments(3);
            var formatArguments2 = arguments.FormatArguments(2);
            ushort duration = 0;
            
            switch (arguments.At(0))
            {
                case "user":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: hbc user (player id / name) (time) (message)";
                        return false;
                    }

                    var ply = Player.Get(arguments.At(1));
                    if (ply == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(2), out duration))
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    ply.ShowHint(formatArguments3, duration);
                    
                    response = $"Hint sent to {ply.Nickname}";
                    return true;
                case "users":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: hbc users (player id / name group (i.e.: 1,2,3 or hello,there,hehe)) (time) (message)";
                        return false;
                    }

                    var users = arguments.At(1).Split(',');
                    List<Player> plyList = new();
                    
                    foreach (var s in users)
                    {
                        if (int.TryParse(s, out var id) && Player.Get(id) != null)
                            plyList.Add(Player.Get(id));
                        else if (Player.Get(s) != null)
                            plyList.Add(Player.Get(s));
                    }

                    if (!ushort.TryParse(arguments.At(2), out duration))
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (var p in plyList)
                        p.ShowHint(formatArguments3, duration);


                    var builder = StringBuilderPool.Shared.Rent("Hint sent to players: ");
                    foreach (var p in plyList)
                    {
                        builder.Append("\"");
                        builder.Append(p.Nickname);
                        builder.Append("\"");
                        builder.Append(" ");
                    }
                    
                    var message = builder.ToString();
                    StringBuilderPool.Shared.Return(builder);
                    
                    response = message;
                    return true;
                case "group":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: hbc group (group) (time) (message)";
                        return false;
                    }

                    var broadcastGroup = ServerStatic.PermissionsHandler.GetGroup(arguments.At(1));
                    if (broadcastGroup == null)
                    {
                        response = $"Invalid group: {arguments.At(1)}";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(2), out duration))
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (var player in Player.List)
                    {
                        if (player.Group.BadgeText.Equals(broadcastGroup.BadgeText))
                            player.ShowHint(formatArguments3, duration);
                    }

                    response = $"Hint sent to all members of \"{arguments.At(1)}\"";
                    return true;
                case "groups":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: hbc groups (list of groups (i.e.: owner,admin,moderator)) (time) (message)";
                        return false;
                    }

                    var groups = arguments.At(1).Split(',');
                    List<string> groupList = new();
                    
                    foreach (var s in groups)
                    {
                        var broadGroup = ServerStatic.PermissionsHandler.GetGroup(s);
                        
                        if (broadGroup != null)
                            groupList.Add(broadGroup.BadgeText);

                    }

                    if (!ushort.TryParse(arguments.At(2), out duration))
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (var p in Player.List)
                    {
                        if (groupList.Contains(p.Group.BadgeText))
                            p.ShowHint(formatArguments3, duration);
                    }

                    var bdr = StringBuilderPool.Shared.Rent("Hint sent to groups with badge text: ");
                    foreach (var p in groupList)
                    {
                        bdr.Append("\"");
                        bdr.Append(p);
                        bdr.Append("\"");
                        bdr.Append(" ");
                    }
                    
                    var ms = bdr.ToString();
                    StringBuilderPool.Shared.Return(bdr);
                    
                    response = ms;
                    return true;
                case "role":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: hbc role (RoleTypeId) (time) (message)";
                        return false;
                    }

                    if (!Enum.TryParse(arguments.At(1), true, out RoleTypeId role))
                    {
                        response = $"Invalid value for RoleTypeId: {arguments.At(1)}";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(2), out duration))
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (var player in Player.List)
                    {
                        if (player.Role == role)
                            player.ShowHint(formatArguments3, duration);
                    }

                    response = $"Hint sent to all members of \"{arguments.At(1)}\"";
                    return true;
                case "roles":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: hbc roles (RoleTypeId group (i.e.: ClassD, Scientist, NtfCadet)) (time) (message)";
                        return false;
                    }

                    var roles = arguments.At(1).Split(',');
                    List<RoleTypeId> roleList = new();
                    foreach (var s in roles)
                    {
                        if (Enum.TryParse(s, true, out RoleTypeId r))
                            roleList.Add(r);
                    }

                    if (!ushort.TryParse(arguments.At(2), out duration))
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (var p in Player.List)
                        if (roleList.Contains(p.Role))
                            p.ShowHint(formatArguments3, duration);

                    var build = StringBuilderPool.Shared.Rent("Hint sent to roles: ");
                    foreach (var ro in roleList)
                    {
                        build.Append("\"");
                        build.Append(ro.ToString());
                        build.Append("\"");
                        build.Append(" ");
                    }
                    
                    var msg = build.ToString();
                    StringBuilderPool.Shared.Return(build);
                    
                    response = msg;
                    return true;
                case "random":
                case "someone":
                    if (arguments.Count < 3)
                    {
                        response = "Usage: hbc (random / someone) (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(1), out duration))
                    {
                        response = $"Invalid value for duration: {arguments.At(1)}";
                        return false;
                    }

                    var plyr = Player.List.ToList()[Plugin.NumGen.Next(0, Player.List.Count())];
                    plyr.ShowHint(formatArguments2, duration);
                    
                    response = $"Hint sent to {plyr.Nickname}";
                    return true;
                case "staff":
                case "admin":
                    if (arguments.Count < 3)
                    {
                        response = "Usage: hbc (staff / admin) (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(1), out duration))
                    {
                        response = $"Invalid value for hint broadcast time: {arguments.At(1)}";
                        return false;
                    }

                    foreach (var pl in Player.List)
                    {
                        if (pl.ReferenceHub.serverRoles.RemoteAdmin)
                            pl.ShowHint($"<color=orange>[Admin Hint]</color> <color=green>{formatArguments2} - {((CommandSender)sender).Nickname}</color>", duration);
                    }

                    response = $"Hint sent to all currently online staff";
                    return true;
                case "clearall":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: hbc clearall";
                        return false;
                    }

                    foreach (var py in Player.List)
                        py.ShowHint(" ");
                    
                    response = "All hints have been cleared";
                    return true;
                default:
                    if (arguments.Count < 3)
                    {
                        response = "Usage: hbc (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(1), out duration))
                    {
                        response = $"Invalid value for hint broadcast time: {arguments.At(0)}";
                        return false;
                    }

                    foreach (var py in Player.List)
                    {
                        if (py.IPAddress != "127.0.0.1")
                            py.ShowHint(formatArguments2, duration);
                    }

                    break;
            }
            response = "";
            return false;
        }
    }
}
