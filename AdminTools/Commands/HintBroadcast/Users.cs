using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommandSystem;
using Exiled.API.Features;
using NorthwoodLib.Pools;

namespace AdminTools.Commands.HintBroadcast;

public class Users : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (arguments.Count < 3)
        {
            response = "Usage: hbc users (player id / player ids (i.e.: 1,2,3)) (time) (message)";
            return false;
        }

        string[] users = arguments.At(0).Split(',');
        List<Player> plyList = new();
        foreach (string s in users)
        {
            if (int.TryParse(s, out int id) && Player.Get(id) != null)
                plyList.Add(Player.Get(id));
            else if (Player.Get(s) != null)
                plyList.Add(Player.Get(s));
        }

        if (!ushort.TryParse(arguments.At(1), out ushort tme) && tme <= 0)
        {
            response = $"Invalid value for duration: {arguments.At(1)}";
            return false;
        }

        foreach (Player p in plyList)
            p.ShowHint(Extensions.FormatArguments(arguments, 2), tme);


        StringBuilder builder = StringBuilderPool.Shared.Rent("Hint sent to players: ");
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
    }

    public string Command { get; } = "users";
    public string[] Aliases { get; }
    public string Description { get; } = "Sends a broadcast to multiple users";
}