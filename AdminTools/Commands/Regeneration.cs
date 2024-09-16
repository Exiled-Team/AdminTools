using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using InventorySystem.Items.Usables;
using UnityEngine;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Regeneration : ICommand, IUsageProvider
    {
        public string Command { get; } = "regeneration";

        public string[] Aliases { get; } = { "reg" };

        public string Description { get; } = "Manages regeneration properties for users";
        public string[] Usage { get; } = { "%player%", "duration", "rate" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.reg"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 3)
            {
                response = "Usage: regeneration ((player id / name) or (all / *)) (duration) (rate)";
                return false;
            }

            if (float.TryParse(arguments.At(1), out float duration))
            {
                response = $"Duration should be a float number: {arguments.At(1)}";
                return false;
            }

            if (float.TryParse(arguments.At(2), out float rate))
            {
                response = $"Rate should be a float number: {arguments.At(1)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            AnimationCurve animationCurve = AnimationCurve.Constant(0f, duration, rate);
            RegenerationProcess reg = new(animationCurve, 1f, 1f);
            foreach (Player player in players)
                UsableItemsController.GetHandler(player.ReferenceHub).ActiveRegenerations.Add(reg);
            response = $"The specified player has been regenerated:\n{players.LogPlayers()}";
            return true;
        }
    }
}