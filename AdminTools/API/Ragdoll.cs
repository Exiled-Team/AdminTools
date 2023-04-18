using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;
using ExiledRagdoll = Exiled.API.Features.Ragdoll;

namespace AdminTools.API
{
    public class Ragdoll
    {
        public static IEnumerator<float> SpawnBodies(Player player, RoleTypeId role, int count)
        {
            for (var i = 0; i < count; i++)
            {
                ExiledRagdoll.CreateAndSpawn(role, "SCP-343", "End of the Universe", player.Position, Quaternion.identity);
                yield return Timing.WaitForSeconds(0.15f);
            }
        }
    }
}