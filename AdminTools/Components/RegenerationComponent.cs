using System;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using Exiled.API.Features;

namespace AdminTools
{
    public class RegenerationComponent : MonoBehaviour
    {
        private Player _player;
        CoroutineHandle _handle;
        public void Awake()
        {
            _player = Player.Get(gameObject);
            _handle = Timing.RunCoroutine(HealHealth(_player));
            Main.RgnHubs.Add(_player, this);
        }

        public void OnDestroy()
        {
            Timing.KillCoroutines(_handle);
            Main.RgnHubs.Remove(_player);
        }

        public IEnumerator<float> HealHealth(Player ply)
        {
            while (true)
            {
                if (ply.Health < ply.MaxHealth)
                    ply.Health += Main.HealthGain;
                else
                    ply.Health = ply.MaxHealth;

                yield return Timing.WaitForSeconds(Main.HealthInterval);
            }
        }
    }
}