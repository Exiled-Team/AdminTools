using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Exiled.API.Features;

namespace AdminTools.Commands.HintBroadcast;

public class Clearall : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        foreach (Player py in Player.List)
            py.ShowHint(" ");
        response = "All hints have been cleared";
        return true;     
    }

    public string Command { get; } = "clearall";
    public string[] Aliases { get; }
    public string Description { get; } = "Clears all broadcasts";
}