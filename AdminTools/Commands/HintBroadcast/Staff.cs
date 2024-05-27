using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Exiled.API.Features;

namespace AdminTools.Commands.HintBroadcast;

public class Staff : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (arguments.Count < 2)
        {
            response = "Usage: hbc (staff / admin) (time) (message)";
            return false;
        }

        if (!ushort.TryParse(arguments.At(0), out ushort t))
        {
            response = $"Invalid value for hint broadcast time: {arguments.At(1)}";
            return false;
        }

        foreach (Player pl in Player.List)
        {
            if (pl.ReferenceHub.serverRoles.RemoteAdmin)
                pl.ShowHint($"<color=orange>[Admin Hint]</color> <color=green>{Extensions.FormatArguments(arguments, 1)} - {((CommandSender)sender).Nickname}</color>", t);
        }

        response = "Hint sent to all currently online staff";
        return true;
    }

    public string Command { get; } = "staff";
    public string[] Aliases { get; } = new[] { "admins" };
    public string Description { get; } = "Sends a broadcast to all online staff members";
}