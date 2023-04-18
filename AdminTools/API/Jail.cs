using System;
using System.Collections.Generic;
using System.Linq;
using AdminTools.API.Entities;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace AdminTools.API
{
    public static class Jail
    {
        internal static readonly HashSet<Jailed> JailedPlayers = new();
    
        public static IEnumerator<float> JailPlayer(Player player, bool skipadd = false)
        {
            var ammo = player.Ammo
                .ToDictionary(ammoInfo => ammoInfo.Key.GetAmmoType(),
                    kvp => kvp.Value);

            var items = player.Items.ToList();

            if (!skipadd)
            {
                JailedPlayers.Add(new Jailed
                {
                    Health = player.Health,
                    Position = player.Position,
                    Items = items,
                    Role = player.Role,
                    UserId = player.UserId,
                    CurrentRound = true,
                    Ammo = ammo
                });
            }

            if (player.IsOverwatchEnabled)
                player.IsOverwatchEnabled = false;
        
            yield return Timing.WaitForSeconds(1f);
        
            player.ClearInventory(false);
            player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);
            player.Position = new Vector3(53f, 1020f, -44f);
        }

        public static IEnumerator<float> UnjailPlayer(Player player)
        {
            var jailed = JailedPlayers.FirstOrDefault(j => j.UserId == player.UserId);

            if (jailed == null)
            {
                Log.Error("Player not jailed!");
                yield break;
            }

            if (jailed.CurrentRound)
            {
                player.Role.Set(jailed.Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
                yield return Timing.WaitForSeconds(0.5f);
            
                try
                {
                    player.ResetInventory(jailed.Items);
                    player.Health = jailed.Health;
                    player.Position = jailed.Position;
                
                    foreach (var kvp in jailed.Ammo)
                        player.Ammo[kvp.Key.GetItemType()] = kvp.Value;
                }
                catch (Exception e)
                {
                    Log.Error($"{nameof(UnjailPlayer)}: {e}");
                }
            }
            else
            {
                player.Role.Set(RoleTypeId.Spectator);
            }

            JailedPlayers.Remove(jailed);
        }
    }
}