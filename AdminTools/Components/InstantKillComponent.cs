using Exiled.API.Features;
using Exiled.Events.EventArgs;
using UnityEngine;
using Handlers = Exiled.Events.Handlers;

namespace AdminTools
{
    using Exiled.Events.EventArgs.Player;
    using PlayerStatsSystem;

    public class InstantKillComponent : MonoBehaviour
    {
        public Player Player;
        public void Awake()
        {
            Player = Player.Get(gameObject);
            Handlers.Player.Hurting += RunWhenPlayerIsHurt;
            Handlers.Player.Left += OnLeave;
            Main.IkHubs.Add(Player, this);
        }

        private void OnLeave(LeftEventArgs ev)
        {
            if (ev.Player == Player)
                Destroy(this);
        }

        public void OnDestroy()
        {
            Handlers.Player.Hurting -= RunWhenPlayerIsHurt;
            Handlers.Player.Left -= OnLeave;
            Main.IkHubs.Remove(Player);
        }

        public void RunWhenPlayerIsHurt(HurtingEventArgs ev)
        {
            if (ev.Attacker != ev.Player && ev.Attacker == Player)
                ev.Amount = StandardDamageHandler.KillValue;
        }
    }
}