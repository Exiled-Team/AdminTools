namespace AdminTools.API
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using MEC;
    using PlayerRoles;
    using UnityEngine;

    public static class Rocket
    {
        public static IEnumerator<float> DoRocket(Player player, float speed)
        {
            int i = 0;
            while (player.Role != RoleTypeId.Spectator)
            {
                player.Position += Vector3.up * speed;
                i++;

                if (i >= 50)
                {
                    player.IsGodModeEnabled = false;

                    ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                    grenade.FuseTime = 0.5f;
                    grenade.SpawnActive(player.Position, player);

                    player.Kill("Went on a trip in their favorite rocket ship.");
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}