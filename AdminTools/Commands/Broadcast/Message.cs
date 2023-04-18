using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using NorthwoodLib.Pools;
using PlayerRoles;
using RemoteAdmin;

namespace AdminTools.Commands.Broadcast
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Message : ParentCommand
    {
        public Message() => LoadGeneratedCommands();

        public override string Command { get; } = "atbroadcast";

        public override string[] Aliases { get; } = new string[] { "atbc" };

        public override string Description { get; } = "Broadcasts a message to either a user, a group, a role, all staff, or everyone";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "broadcast", PlayerPermissions.Broadcasting, "AdminTools", false))
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
            
            switch (arguments.At(0))
            {
                case "user":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast user (player id / name) (time) (message)";
                        return false;
                    }

                    var ply = Player.Get(arguments.At(1));
                    if (ply == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(2), out var time) && time <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    ply.Broadcast(time, EventHandlers.FormatArguments(arguments, 3));
                    response = $"Message sent to {ply.Nickname}";
                    return true;
                case "users":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast users (player id / name group (i.e.: 1,2,3 or hello,there,hehe)) (time) (message)";
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

                    if (!ushort.TryParse(arguments.At(2), out var tme) && tme <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (var p in plyList)
                        p.Broadcast(tme, EventHandlers.FormatArguments(arguments, 3));


                    var builder = StringBuilderPool.Shared.Rent("Message sent to players: ");
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
                        response = "Usage: broadcast group (group) (time) (message)";
                        return false;
                    }

                    var broadcastGroup = ServerStatic.PermissionsHandler.GetGroup(arguments.At(1));
                    if (broadcastGroup == null)
                    {
                        response = $"Invalid group: {arguments.At(1)}";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(2), out var tim) && tim <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (var player in Player.List)
                    {
                        if (player.Group.BadgeText.Equals(broadcastGroup.BadgeText))
                            player.Broadcast(tim, EventHandlers.FormatArguments(arguments, 3));
                    }

                    response = $"Message sent to all members of \"{arguments.At(1)}\"";
                    return true;
                case "groups":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast groups (list of groups (i.e.: owner,admin,moderator)) (time) (message)";
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

                    if (!ushort.TryParse(arguments.At(2), out var e) && e <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (var p in Player.List)
                        if (groupList.Contains(p.Group.BadgeText))
                            p.Broadcast(e, EventHandlers.FormatArguments(arguments, 3));


                    var bdr = StringBuilderPool.Shared.Rent("Message sent to groups with badge text: ");
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
                        response = "Usage: broadcast role (RoleTypeId) (time) (message)";
                        return false;
                    }

                    if (!Enum.TryParse(arguments.At(1), true, out RoleTypeId role))
                    {
                        response = $"Invalid value for RoleTypeId: {arguments.At(1)}";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(2), out var te) && te <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (var player in Player.List)
                    {
                        if (player.Role == role)
                            player.Broadcast(te, EventHandlers.FormatArguments(arguments, 3));
                    }

                    response = $"Message sent to all members of \"{arguments.At(1)}\"";
                    return true;
                case "roles":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast roles (RoleTypeId group (i.e.: ClassD, Scientist, NtfCadet)) (time) (message)";
                        return false;
                    }

                    var roles = arguments.At(1).Split(',');
                    List<RoleTypeId> roleList = new();
                    foreach (var s in roles)
                    {
                        if (Enum.TryParse(s, true, out RoleTypeId r))
                            roleList.Add(r);
                    }

                    if (!ushort.TryParse(arguments.At(2), out var ti) && ti <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (var p in Player.List)
                        if (roleList.Contains(p.Role) && p.ReferenceHub.queryProcessor._ipAddress != "127.0.0.1")
                            p.Broadcast(ti, EventHandlers.FormatArguments(arguments, 3));

                    var build = StringBuilderPool.Shared.Rent("Message sent to roles: ");
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
                        response = "Usage: broadcast (random / someone) (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(1), out var me) && me <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(1)}";
                        return false;
                    }

                    var plyr = Player.List.ToList()[Plugin.NumGen.Next(0, Player.List.Count())];
                    if (plyr.ReferenceHub.queryProcessor._ipAddress != "127.0.0.1")
                        plyr.Broadcast(me, EventHandlers.FormatArguments(arguments, 2));
                    response = $"Message sent to {plyr.Nickname}";
                    return true;
                case "staff":
                case "admin":
                    if (arguments.Count < 3)
                    {
                        response = "Usage: broadcast (staff / admin) (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(1), out var t))
                    {
                        response = $"Invalid value for broadcast time: {arguments.At(1)}";
                        return false;
                    }

                    foreach (var pl in Player.List)
                    {
                        if (pl.ReferenceHub.serverRoles.RemoteAdmin)
                            pl.Broadcast(t, EventHandlers.FormatArguments(arguments, 2) + $" - {((CommandSender)sender).Nickname}", global::Broadcast.BroadcastFlags.AdminChat);
                    }

                    response = $"Message sent to all currently online staff";
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

                    if (!ushort.TryParse(arguments.At(0), out var tm))
                    {
                        response = $"Invalid value for broadcast time: {arguments.At(0)}";
                        return false;
                    }
                    Map.Broadcast(tm, EventHandlers.FormatArguments(arguments, 1));
                    break;
            }
            response = "";
            return false;
        }
    }
}
