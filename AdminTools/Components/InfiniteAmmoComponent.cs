using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;

namespace AdminTools
{
    public class InfiniteAmmoComponent : MonoBehaviour
    {
        private Player player;
        CoroutineHandle Handle;
        
                public void Awake()
        {
            RegisterEvents();
            player = Player.Get(gameObject);
            Handle = Timing.RunCoroutine(Ammo());
            Plugin.IaHubs.Add(player, this);
        }

        public void Start()
        {
            
        }
        
        private void Update()
        {
            if (player == null)
            {
                Destroy();
                return;
            }
        }

        private void OnShoot(ShootingEventArgs ev)
        {
            if (player.CurrentItem.id.IsWeapon())
            {
                SetAmmo(player, player.CurrentItem, 100);
            }
        }
        private void SetAmmo(Player player, Inventory.SyncItemInfo weapon, int value)
        {
            player.SetWeaponAmmo(weapon, value);
        }

        private void ResetAmmo(Player player)
        {
            player.Ammo[(int) AmmoType.Nato9] = 0;
            player.Ammo[(int) AmmoType.Nato762] = 0;
            player.Ammo[(int) AmmoType.Nato556] = 0;

        }

        public IEnumerator<float> Ammo()
        {
            while (player.CurrentItem.id.IsWeapon())
            {
                SetAmmo(player, player.CurrentItem, 100);
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
        private void OnDestroy() => PartiallyDestroy();
        public void PartiallyDestroy()
        {
            UnregisterEvents();
            Timing.KillCoroutines(Handle);
            ResetAmmo(player);
            if (player.CurrentItem.id.IsWeapon())
            {
                SetAmmo(player, player.CurrentItem, 40);
            }
            if (player == null)
                return;
        }

        public void Destroy()
        {
            try
            {
                Destroy(this);
            }
            catch (Exception exception)
            {
                Log.Error($"Error!: {exception}");
            }
        }

        private void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShoot;
        }

        private void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Shooting -= OnShoot;
        }
    }
}