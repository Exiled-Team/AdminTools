namespace AdminTools.Components
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using UnityEngine;

    public class RegenerationComponent : MonoBehaviour
    {
        public static readonly Dictionary<Player, RegenerationComponent> RegeneratingHubs = new();

        public static float HealthGain = 5;

        public static float HealthInterval = 1;

        private CoroutineHandle _handle;

        private Player _player;

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