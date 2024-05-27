using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommandSystem;
using Exiled.API.Features;
using NorthwoodLib.Pools;

namespace AdminTools.Commands.HintBroadcast;

public class Groups : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count < 3)
        {
            response = "Usage: hbc groups (list of groups (i.e.: owner,admin,moderator)) (time) (message)";
            return false;
        }

        string[] groups = arguments.At(0).Split(',');
        List<string> groupList = new();
        foreach (string s in groups)
        {
            UserGroup broadGroup = ServerStatic.PermissionsHandler.GetGroup(s);
            if (broadGroup != null)
                groupList.Add(broadGroup.BadgeText);

        }

        if (!ushort.TryParse(arguments.At(1), out ushort e) && e <= 0)
        {
            response = $"Invalid value for duration: {arguments.At(1)}";
            return false;
        }

        foreach (Player p in Player.List)
            if (groupList.Contains(p.Group.BadgeText))
                p.ShowHint(Extensions.FormatArguments(arguments, 2), e);


        StringBuilder bdr = StringBuilderPool.Shared.Rent("Hint sent to groups with badge text: ");
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
    }

    public string Command { get; } = "groups";
    public string[] Aliases { get; }
    public string Description { get; } = "Sends a broadcast to multiple groups";
}