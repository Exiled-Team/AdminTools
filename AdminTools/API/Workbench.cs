using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AdminTools.API
{
    public static class Workbench
    {
        public static void SpawnWorkbench(Player player, Vector3 position, Vector3 rotation, Vector3 size, out int benchIndex)
        {
            try
            {
                Log.Debug($"Spawning workbench");
            
                benchIndex = 0;
                var bench = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Work Station"));
            
                rotation.x += 180;
                rotation.z += 180;
            
                Offset offset = new()
                {
                    position = position,
                    rotation = rotation,
                    scale = Vector3.one
                };
            
                bench.gameObject.transform.localScale = size;
                NetworkServer.Spawn(bench);
            
                if (Plugin.SpawnedBenchHubs.TryGetValue(player, out var objs))
                {
                    objs.Add(bench);
                }
                else
                {
                    Plugin.SpawnedBenchHubs.Add(player, new List<GameObject>());
                    Plugin.SpawnedBenchHubs[player].Add(bench);
                
                    benchIndex = Plugin.SpawnedBenchHubs[player].Count();
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