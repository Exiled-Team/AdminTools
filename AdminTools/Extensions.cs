using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AdminTools
{
    /// <summary>
    /// Provides extension methods for various game-related functionalities.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Formats the arguments of a sentence starting from the specified index.
        /// </summary>
        /// <param name="sentence">The array segment containing the sentence.</param>
        /// <param name="index">The starting index for formatting.</param>
        /// <returns>A formatted string of the sentence from the specified index.</returns>
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

        /// <summary>
        /// Logs the players with their nickname and ID.
        /// </summary>
        /// <param name="players">The collection of players to log.</param>
        /// <returns>A string containing player nicknames and IDs.</returns>
        public static string LogPlayers(this IEnumerable<Player> players) =>
            string.Join("\n - ", players.Select(x => $"{x.Nickname}({x.Id})"));

        /// <summary>
        /// Saves the player's overwatch state to the configuration.
        /// </summary>
        /// <param name="player">The player whose data is being saved.</param>
        public static void SavingPlayerData(this Player player)
        {
            List<string> overwatchRead = Main.Overwatch;

            string userId = player.UserId;

            if (player.IsOverwatchEnabled && !overwatchRead.Contains(userId))
            {
                overwatchRead.Add(userId);
                Log.Debug($"{player.Nickname}({player.UserId}) has added their overwatch.");
            }
            else if (!player.IsOverwatchEnabled && overwatchRead.Remove(userId))
                Log.Debug($"{player.Nickname}({player.UserId}) has removed their overwatch.");
        }

        /// <summary>
        /// Spawns a workbench at the specified position, rotation, and size.
        /// </summary>
        /// <param name="ply">The player who is spawning the workbench.</param>
        /// <param name="position">The position to spawn the workbench at.</param>
        /// <param name="rotation">The rotation of the workbench.</param>
        /// <param name="size">The size of the workbench.</param>
        /// <param name="benchIndex">The index of the spawned workbench in the player's workbench list.</param>
        public static void SpawnWorkbench(Player ply, Vector3 position, Vector3 rotation, Vector3 size, out int benchIndex)
        {
            try
            {
                Log.Debug("Spawning workbench");
                benchIndex = 0;
                GameObject bench =
                    Object.Instantiate(
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
