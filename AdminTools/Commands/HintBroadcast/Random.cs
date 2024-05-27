using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;

namespace AdminTools.Commands.HintBroadcast;

public class Random : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (arguments.Count < 2)
        {
            response = "Usage: hbc (random / someone) (time) (message)";
            return false;
        }

        Player ply = Player.List.GetRandomValue();

        if (!ushort.TryParse(arguments.At(0), out ushort time) && time <= 0)
        {
            response = $"Invalid value for duration: {arguments.At(0)}";
            return false;
        }

        ply.ShowHint(Extensions.FormatArguments(arguments, 2), time);
        response = $"Hint sent to {ply.Nickname}";
        return true;
    }
    // time message
    public string Command { get; } = "random";
    public string[] Aliases { get; } = new[] { "someone" };
    public string Description { get; } = "Sends a broadcast to a random user";
}