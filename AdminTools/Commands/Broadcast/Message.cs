﻿namespace AdminTools.Commands.Broadcast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CommandSystem;
    using Exiled.API.Features;
    using Extensions;
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using RemoteAdmin;
    using Broadcast = global::Broadcast;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Message : ICommand
    {
        public string Command => "atbroadcast";

        public string[] Aliases { get; } = { "atbc" };

        public string Description => "Broadcasts a message to either a user, a group, a role, all staff, or everyone";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions((CommandSender)sender, "broadcast",
                    PlayerPermissions.Broadcasting, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\nbroadcast (time) (message)" +
                    "\nbroadcast user (player id / name) (time) (message)" +
                    "\nbroadcast users (player id / name group (i.e.: 1,2,3 or hello,there,hehe)) (time) (message)" +
                    "\nbroadcast group (group name) (time) (message)" +
                    "\nbroadcast groups (list of groups (i.e.: owner,admin,moderator)) (time) (message)" +
                    "\nbroadcast role (RoleTypeId) (time) (message)" +
                    "\nbroadcast roles (RoleTypeId group (i.e.: ClassD,Scientist,NtfCadet)) (time) (message)" +
                    "\nbroadcast (random / someone) (time) (message)" +
                    "\nbroadcast (staff / admin) (time) (message)" +
                    "\nbroadcast clearall";
                return false;
            }

            string formatArguments3 = arguments.FormatArguments(3);
            ushort duration;

            switch (arguments.At(0))
            {
                case "user":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast user (player id / name) (time) (message)";
                        return false;
                    }

                    Player ply = Player.Get(arguments.At(1));
                    if (ply == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(2), out duration) && duration <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    ply.Broadcast(duration, formatArguments3);
                    response = $"Message sent to {ply.Nickname}";
                    return true;
                case "users":
                    if (arguments.Count < 4)
                    {
                        response =
                            "Usage: broadcast users (player id / name group (i.e.: 1,2,3 or hello,there,hehe)) (time) (message)";
                        return false;
                    }

                    string[] users = arguments.At(1).Split(',');
                    List<Player> plyList = new();

                    foreach (string s in users)
                    {
                        if (int.TryParse(s, out int id) && Player.Get(id) != null)
                            plyList.Add(Player.Get(id));
                        else if (Player.Get(s) != null)
                            plyList.Add(Player.Get(s));
                    }

                    if (!ushort.TryParse(arguments.At(2), out ushort tme) && tme <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (Player p in plyList)
                    {
                        p.Broadcast(tme, formatArguments3);
                    }

                    StringBuilder builder = StringBuilderPool.Shared.Rent("Message sent to players: ");
                    foreach (Player p in plyList)
                    {
                        builder.Append("\"");
                        builder.Append(p.Nickname);
                        builder.Append("\"");
                        builder.Append(" ");
                    }

                    string message = builder.ToString();
                    StringBuilderPool.Shared.Return(builder);

                    response = message;
                    return true;
                case "group":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast group (group) (time) (message)";
                        return false;
                    }

                    UserGroup broadcastGroup = ServerStatic.PermissionsHandler.GetGroup(arguments.At(1));
                    if (broadcastGroup == null)
                    {
                        response = $"Invalid group: {arguments.At(1)}";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(2), out ushort tim) && tim <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (Player player in Player.List)
                    {
                        if (player.Group.BadgeText.Equals(broadcastGroup.BadgeText))
                            player.Broadcast(tim, formatArguments3);
                    }

                    response = $"Message sent to all members of \"{arguments.At(1)}\"";
                    return true;
                case "groups":
                    if (arguments.Count < 4)
                    {
                        response =
                            "Usage: broadcast groups (list of groups (i.e.: owner,admin,moderator)) (time) (message)";
                        return false;
                    }

                    string[] groups = arguments.At(1).Split(',');
                    List<string> groupList = new();

                    foreach (string s in groups)
                    {
                        UserGroup broadGroup = ServerStatic.PermissionsHandler.GetGroup(s);
                        if (broadGroup != null)
                            groupList.Add(broadGroup.BadgeText);
                    }

                    if (!ushort.TryParse(arguments.At(2), out ushort e) && e <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (Player p in Player.List)
                    {
                        if (groupList.Contains(p.Group.BadgeText))
                            p.Broadcast(e, formatArguments3);
                    }


                    StringBuilder bdr = StringBuilderPool.Shared.Rent("Message sent to groups with badge text: ");
                    foreach (string p in groupList)
                    {
                        bdr.Append("\"");
                        bdr.Append(p);
                        bdr.Append("\"");
                        bdr.Append(" ");
                    }

                    string ms = bdr.ToString();
                    StringBuilderPool.Shared.Return(bdr);

                    response = ms;
                    return true;
                case "role":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast role (RoleTypeId) (time) (message)";
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

                    foreach (Player player in Player.List)
                    {
                        if (player.Role == role)
                            player.Broadcast(duration, formatArguments3);
                    }

                    response = $"Message sent to all members of \"{arguments.At(1)}\"";
                    return true;
                case "roles":
                    if (arguments.Count < 4)
                    {
                        response =
                            "Usage: broadcast roles (RoleTypeId group (i.e.: ClassD, Scientist, NtfCadet)) (time) (message)";
                        return false;
                    }

                    string[] roles = arguments.At(1).Split(',');
                    List<RoleTypeId> roleList = new();

                    foreach (string s in roles)
                    {
                        if (Enum.TryParse(s, true, out RoleTypeId r))
                            roleList.Add(r);
                    }

                    if (!ushort.TryParse(arguments.At(2), out duration))
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (Player p in Player.List)
                    {
                        if (roleList.Contains(p.Role) && p.IPAddress != "127.0.0.1")
                            p.Broadcast(duration, formatArguments3);
                    }

                    StringBuilder build = StringBuilderPool.Shared.Rent("Message sent to roles: ");
                    foreach (RoleTypeId ro in roleList)
                    {
                        build.Append("\"");
                        build.Append(ro.ToString());
                        build.Append("\"");
                        build.Append(" ");
                    }

                    string msg = build.ToString();
                    StringBuilderPool.Shared.Return(build);
                    response = msg;

                    return true;
                case "random":
                case "someone":
                    if (arguments.Count < 3)
                    {
                        response = "Usage: broadcast (random / someone) (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(1), out duration))
                    {
                        response = $"Invalid value for duration: {arguments.At(1)}";
                        return false;
                    }

                    Player plyr = Player.List.ToList()[Plugin.NumGen.Next(0, Player.List.Count())];

                    if (plyr.IPAddress != "127.0.0.1")
                        plyr.Broadcast(duration, formatArguments3);

                    response = $"Message sent to {plyr.Nickname}";
                    return true;
                case "staff":
                case "admin":
                    if (arguments.Count < 3)
                    {
                        response = "Usage: broadcast (staff / admin) (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(1), out duration))
                    {
                        response = $"Invalid value for broadcast time: {arguments.At(1)}";
                        return false;
                    }

                    foreach (Player pl in Player.List)
                    {
                        if (pl.ReferenceHub.serverRoles.RemoteAdmin)
                            pl.Broadcast(duration,
                                arguments.FormatArguments(2) + $" - {((CommandSender)sender).Nickname}",
                                Broadcast.BroadcastFlags.AdminChat);
                    }

                    response = "Message sent to all currently online staff";
                    return true;
                case "clearall":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: broadcast clearall";
                        return false;
                    }

                    Map.ClearBroadcasts();
                    response = "All current broadcasts have been cleared";
                    return true;
                default:
                    if (arguments.Count < 2)
                    {
                        response = "Usage: broadcast (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(0), out duration))
                    {
                        response = $"Invalid value for broadcast time: {arguments.At(0)}";
                        return false;
                    }

                    Map.Broadcast(duration, arguments.FormatArguments(1));
                    break;
            }

            response = "";
            return false;
        }
    }
}