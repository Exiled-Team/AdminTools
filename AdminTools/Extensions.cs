using Exiled.API.Features;
using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AdminTools
{
    public static class Extensions
    {
        public static string FormatArguments(this ArraySegment<string> sentence, int index)
        {
            StringBuilder sb = new();
            foreach (string word in sentence.Segment(index))
            {
                sb.Append(word);
                sb.Append(" ");
            }
            string msg = sb.ToString();
            return msg;
        }

        public static string LogPlayers(this IEnumerable<Player> players) => string.Join("\n - ", players.Select(x => $"{x.Nickname}({x.Id})"));

        public static void SavingPlayerData(this Player player)
        {
            List<string> overwatchRead = Main.Overwatch;
            List<string> tagsRead = Main.HiddenTags;

            string userId = player.UserId;

            if (player.IsOverwatchEnabled && !overwatchRead.Contains(userId))
            {
                overwatchRead.Add(userId);
                Log.Debug($"{player.Nickname}({player.UserId}) has added their overwatch.");
            }
            else if (!player.IsOverwatchEnabled && overwatchRead.Remove(userId))
                Log.Debug($"{player.Nickname}({player.UserId}) has remove their overwatch.");

            if (player.BadgeHidden && !tagsRead.Contains(userId))
            {
                tagsRead.Add(userId);
                Log.Debug($"{player.Nickname}({player.UserId}) has added their tag hidden.");
            }
            else if (!player.BadgeHidden && tagsRead.Remove(userId))
                Log.Debug($"{player.Nickname}({player.UserId}) has remove their tag hidden.");
        }
        public static void SpawnWorkbench(Player ply, Vector3 position, Vector3 rotation, Vector3 size, out int benchIndex)
        {
            try
            {
                Log.Debug($"Spawning workbench");
                benchIndex = 0;
                GameObject bench =
                    UnityEngine.Object.Instantiate(
                        NetworkClient.prefabs.Values.First(x => x.name.Contains("Work Station")));
                rotation.x += 180;
                rotation.z += 180;
                Offset offset = new()
                {
                    position = position,
                    rotation = rotation,
                    scale = Vector3.one,
                };
                bench.gameObject.transform.localScale = size;
                NetworkServer.Spawn(bench);
                if (Main.BchHubs.TryGetValue(ply, out List<GameObject> objs))
                {
                    objs.Add(bench);
                }
                else
                {
                    Main.BchHubs.Add(ply, new());
                    Main.BchHubs[ply].Add(bench);
                    benchIndex = Main.BchHubs[ply].Count();
                }

                if (benchIndex != 1)
                    benchIndex = objs.Count();
                bench.transform.localPosition = offset.position;
                bench.transform.localRotation = Quaternion.Euler(offset.rotation);
                bench.AddComponent<WorkstationController>();
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(SpawnWorkbench)}: {e}");
                benchIndex = -1;
            }
        }
    }
}
