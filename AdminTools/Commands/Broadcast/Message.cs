using CommandSystem;
using Exiled.API.Features;
using NorthwoodLib.Pools;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdminTools.Commands.Message
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Message : ParentCommand
    {
        public Message() => LoadGeneratedCommands();

        public override string Command { get; } = "broadcast";

        public override string[] Aliases { get; } = new string[] { "bc" };

        public override string Description { get; } = "Broadcasts a message to either a user, a group, a role, all staff, or everyone";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            EventHandlers.LogCommandUsed((CommandSender)sender, EventHandlers.FormatArguments(arguments, 0));
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
                    "\nbroadcast role (RoleType) (time) (message)" +
                    "\nbroadcast roles (RoleType group (i.e.: ClassD,Scientist,NtfCadet)) (time) (message)" +
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

                    Player Ply = Player.Get(arguments.At(1));
                    if (Ply == null)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(2), out ushort time) && time <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    Ply.Broadcast(time, EventHandlers.FormatArguments(arguments, 3));
                    response = $"Message sent to {Ply.Nickname}";
                    return true;
                case "users":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast users (player id / name group (i.e.: 1,2,3 or hello,there,hehe)) (time) (message)";
                        return false;
                    }

                    string[] Users = arguments.At(1).Split(',');
                    List<Player> PlyList = new List<Player>();
                    foreach (string s in Users)
                    {
                        if (int.TryParse(s, out int id) && Player.Get(id) != null)
                            PlyList.Add(Player.Get(id));
                        else if (Player.Get(s) != null)
                            PlyList.Add(Player.Get(s));
                    }

                    if (!ushort.TryParse(arguments.At(2), out ushort tme) && tme <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (Player P in PlyList)
                        P.Broadcast(tme, EventHandlers.FormatArguments(arguments, 3));


                    StringBuilder Builder = StringBuilderPool.Shared.Rent("Message sent to players: ");
                    foreach (Player P in PlyList)
                    {
                        Builder.Append("\"");
                        Builder.Append(P.Nickname);
                        Builder.Append("\"");
                        Builder.Append(" ");
                    }
                    string message = Builder.ToString();
                    StringBuilderPool.Shared.Return(Builder);
                    response = message;
                    return true;
                case "group":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast group (group) (time) (message)";
                        return false;
                    }

                    UserGroup BroadcastGroup = ServerStatic.PermissionsHandler.GetGroup(arguments.At(1));
                    if (BroadcastGroup == null)
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
                        if (player.Group.BadgeText.Equals(BroadcastGroup.BadgeText))
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

                    string[] Groups = arguments.At(1).Split(',');
                    List<string> GroupList = new List<string>();
                    foreach (string s in Groups)
                    {
                        UserGroup BroadGroup = ServerStatic.PermissionsHandler.GetGroup(s);
                        if (BroadGroup != null)
                            GroupList.Add(BroadGroup.BadgeText);

                    }

                    if (!ushort.TryParse(arguments.At(2), out ushort e) && e <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (Player P in Player.List)
                        if (GroupList.Contains(P.Group.BadgeText))
                            P.Broadcast(e, EventHandlers.FormatArguments(arguments, 3));


                    StringBuilder Bdr = StringBuilderPool.Shared.Rent("Message sent to groups with badge text: ");
                    foreach (string P in GroupList)
                    {
                        Bdr.Append("\"");
                        Bdr.Append(P);
                        Bdr.Append("\"");
                        Bdr.Append(" ");
                    }
                    string ms = Bdr.ToString();
                    StringBuilderPool.Shared.Return(Bdr);
                    response = ms;
                    return true;
                case "role":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast role (RoleType) (time) (message)";
                        return false;
                    }

                    if (!Enum.TryParse(arguments.At(1), true, out RoleType Role))
                    {
                        response = $"Invalid value for RoleType: {arguments.At(1)}";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(2), out ushort te) && te <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (Player Player in Player.List)
                    {
                        if (Player.Role == Role)
                            Player.Broadcast(te, EventHandlers.FormatArguments(arguments, 3));
                    }

                    response = $"Message sent to all members of \"{arguments.At(1)}\"";
                    return true;
                case "roles":
                    if (arguments.Count < 4)
                    {
                        response = "Usage: broadcast roles (RoleType group (i.e.: ClassD, Scientist, NtfCadet)) (time) (message)";
                        return false;
                    }

                    string[] Roles = arguments.At(1).Split(',');
                    List<RoleType> RoleList = new List<RoleType>();
                    foreach (string s in Roles)
                    {
                        if (Enum.TryParse(s, true, out RoleType R))
                            RoleList.Add(R);
                    }

                    if (!ushort.TryParse(arguments.At(2), out ushort ti) && ti <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(2)}";
                        return false;
                    }

                    foreach (Player P in Player.List)
                        if (RoleList.Contains(P.Role))
                            P.Broadcast(ti, EventHandlers.FormatArguments(arguments, 3));

                    StringBuilder Build = StringBuilderPool.Shared.Rent("Message sent to roles: ");
                    foreach (RoleType Ro in RoleList)
                    {
                        Build.Append("\"");
                        Build.Append(Ro.ToString());
                        Build.Append("\"");
                        Build.Append(" ");
                    }
                    string msg = Build.ToString();
                    StringBuilderPool.Shared.Return(Build);
                    response = msg;
                    return true;
                case "random":
                case "someone":
                    if (arguments.Count < 3)
                    {
                        response = "Usage: broadcast (random / someone) (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(1), out ushort me) && me <= 0)
                    {
                        response = $"Invalid value for duration: {arguments.At(1)}";
                        return false;
                    }

                    Player Plyr = Player.List.ToList()[Plugin.NumGen.Next(0, Player.List.Count())];
                    Plyr.Broadcast(me, EventHandlers.FormatArguments(arguments, 2));
                    response = $"Message sent to {Plyr.Nickname}";
                    return true;
                case "staff":
                case "admin":
                    if (arguments.Count < 3)
                    {
                        response = "Usage: broadcast (staff / admin) (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(1), out ushort t))
                    {
                        response = $"Invalid value for broadcast time: {arguments.At(1)}";
                        return false;
                    }

                    foreach (Player Pl in Player.List)
                    {
                        if (Pl.Sender.CheckPermission(PlayerPermissions.AdminChat))
                            Pl.Broadcast(t, EventHandlers.FormatArguments(arguments, 2) + $" - {((CommandSender)sender).Nickname}", Broadcast.BroadcastFlags.AdminChat);
                    }

                    response = $"Message sent to all currently online staff";
                    return true;
                case "clearall":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: broadcast clearall";
                        return false;
                    }

                    PlayerManager.localPlayer.GetComponent<Broadcast>().RpcClearElements();
                    response = "All current broadcasts have been cleared";
                    return true;
                default:
                    if (arguments.Count < 2)
                    {
                        response = "Usage: broadcast (time) (message)";
                        return false;
                    }

                    if (!ushort.TryParse(arguments.At(0), out ushort tm))
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
