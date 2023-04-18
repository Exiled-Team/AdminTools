using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace AdminTools.Components
{
    public class RegenerationComponent : MonoBehaviour
    {
        private Player _player;
        private CoroutineHandle _handle;
        
        public static readonly Dictionary<Player, RegenerationComponent> RegeneratingHubs = new();
        
        public static float HealthGain = 5;
        public static float HealthInterval = 1;

        public void Awake()
        {
            _player = Player.Get(gameObject);
            _handle = Timing.RunCoroutine(HealHealth(_player));
            
            RegeneratingHubs.Add(_player, this);
        }

        public void OnDestroy()
        {
            Timing.KillCoroutines(_handle);
            RegeneratingHubs.Remove(_player);
        }

        public IEnumerator<float> HealHealth(Player ply)
        {
            while (true)
            {
                if (ply.Health < ply.MaxHealth)
                    ply.Health += HealthGain;
                else
                    ply.Health = ply.MaxHealth;

                yield return Timing.WaitForSeconds(HealthInterval);
            }
        }
    }
}