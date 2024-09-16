using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomModules;
using Exiled.CustomModules.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Jail : ICommand, IUsageProvider
    {
        public string Command { get; } = "jail";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Jails or unjails a user";

        public string[] Usage { get; } = { "%player%", "[IsJail]"};

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.jail"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: jail (player id / name) [true/false]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }
            
            bool? isJail = null;
            if (bool.TryParse(arguments.ElementAtOrDefault(1), out bool result))
                isJail = result;

            foreach (Player ply in players)
            {
                
                if (isJail is true)
                    DoJail(ply);
                else if (isJail is false)
                    DoUnJail(ply);
                else
                {
                    if (Main.JailedPlayers.ContainsKey(ply.UserId))
                        DoUnJail(ply);
                    else
                        DoJail(ply);
                }
            }
            response = $"Jail command has run successfully.\n{players.LogPlayers()}";
            return true;
        }
        public static void DoJail(Player player, bool skipadd = false)
        {
            if (Main.JailedPlayers.ContainsKey(player.UserId))
                return;

            if (!skipadd)
            {
                Main.JailedPlayers.Add(player.UserId, new Jailed
                {
                    Health = player.Health,
                    RelativePosition = player.RelativePosition,
                    Items = CustomModules.IsLoaded ?
                        player.Items.Cast<object>().Concat(player.Cast<Pawn>().CustomItems.Cast<object>()).ToList() :
                        player.Items.Cast<object>().ToList(),
                    Effects = player.ActiveEffects.Select(x => new Effect(x)).ToList(),
                    Name = player.Nickname,
                    Role = CustomModules.IsLoaded && player.Cast<Pawn>().CustomRole is not null ? player.Cast<Pawn>().CustomRole.Id : player.Role.Type,
                    CurrentRound = true,
                    Ammo = player.Ammo.ToDictionary(x => x.Key.GetAmmoType(), x => x.Value),
                    CustomAmmo = CustomModules.IsLoaded ? player.Cast<Pawn>().CustomAmmoBox.ToDictionary(x => x.Key, x => x.Value) : null,
                });
            }

            if (player.IsOverwatchEnabled)
                player.IsOverwatchEnabled = false;

            player.Ammo.Clear();
            player.Inventory.SendAmmoNextFrame = true;

            player.ClearInventory(false);
            player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.UseSpawnpoint);
        }

        public static void DoUnJail(Player player)
        {
            if (!Main.JailedPlayers.TryGetValue(player.UserId, out Jailed jail))
                return;
            if (jail.CurrentRound)
            {
                if (CustomModules.IsLoaded)
                {
                    player.Cast<Pawn>().SetRole(jail.Role, roleSpawnFlags: RoleSpawnFlags.None);
                }
                else
                {
                    if (jail.Role is RoleTypeId role)
                        player.Role.Set(role, RoleSpawnFlags.None);
                }
                try
                {
                    player.ClearInventory();

                    if (CustomModules.IsLoaded)
                    {
                        foreach (object item in jail.Items)
                            player.Cast<Pawn>().AddItem(item);
                    }
                    else
                    {
                        foreach (object item in jail.Items)
                        {
                            if (item is not ItemType itemType)
                                continue;

                            player.AddItem(itemType);
                        }
                    }

                    player.Health = jail.Health;
                    player.Position = jail.RelativePosition.Position;
                    
                    foreach (KeyValuePair<AmmoType, ushort> kvp in jail.Ammo)
                        player.Ammo[kvp.Key.GetItemType()] = kvp.Value;

                    if (jail.CustomAmmo is not null && CustomModules.IsLoaded)
                    {
                        foreach (KeyValuePair<uint, ushort> kvp in jail.CustomAmmo)
                            player.Cast<Pawn>().AddAmmo(kvp.Key, kvp.Value);
                    }

                    player.SyncEffects(jail.Effects);

                    player.Inventory.SendItemsNextFrame = true;
                    player.Inventory.SendAmmoNextFrame = true;
                }
                catch (Exception e)
                {
                    Log.Error($"{nameof(DoUnJail)}: {e}");
                }
            }
            else
            {
                player.Role.Set(RoleTypeId.Spectator);
            }
            Main.JailedPlayers.Remove(player.UserId);
        }
    }
}
