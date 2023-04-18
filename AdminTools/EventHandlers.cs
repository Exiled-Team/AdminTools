using System;
using System.IO;
using System.Linq;
using AdminTools.API;
using AdminTools.Extensions;
using Exiled.API.Features;
using MEC;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using Log = Exiled.API.Features.Log;

namespace AdminTools
{
    public class EventHandlers
    {
        private readonly Config _pluginConfig;

        internal EventHandlers(Config config)
        {
            _pluginConfig = config;
        }

        internal void OnPlayerDestroyed(DestroyingEventArgs ev)
        {
            if (!Plugin.RoundStartMutes.Contains(ev.Player)) return;

            ev.Player.IsMuted = false;
            Plugin.RoundStartMutes.Remove(ev.Player);
        }

        internal void OnPlayerVerified(VerifiedEventArgs ev)
        {
            try
            {
                if (Jail.JailedPlayers.Any(j => j.UserId == ev.Player.UserId))
                    Timing.RunCoroutine(Jail.JailPlayer(ev.Player, true));

                if (_pluginConfig.SaveOverwatchs && File.ReadAllText(Plugin.OverwatchFilePath).Contains(ev.Player.UserId))
                {
                    Log.Debug($"Putting {ev.Player.UserId} into overwatch.");
                    Timing.CallDelayed(1, () => ev.Player.IsOverwatchEnabled = true);
                }

                if (_pluginConfig.SaveHiddenTags && File.ReadAllText(Plugin.HiddenTagsFilePath).Contains(ev.Player.UserId))
                {
                    Log.Debug($"Hiding {ev.Player.UserId}'s tag.");
                    Timing.CallDelayed(1, () => ev.Player.BadgeHidden = true);
                }

                if (Plugin.RoundStartMutes.Count != 0 && !ev.Player.ReferenceHub.serverRoles.RemoteAdmin &&
                    !Plugin.RoundStartMutes.Contains(ev.Player))
                {
                    Log.Debug($"Muting {ev.Player.UserId} (no RA).");
                    ev.Player.IsMuted = true;
                    Plugin.RoundStartMutes.Add(ev.Player);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Player Join: {e}");
            }
        }

        internal void OnRoundStart()
        {
            foreach (var ply in Plugin.RoundStartMutes)
                if (ply != null)
                    ply.IsMuted = false;

            Plugin.RoundStartMutes.Clear();
        }

        internal void OnRoundEnd(RoundEndedEventArgs ev)
        {
            try
            {
                foreach (var jailedPlayer in Jail.JailedPlayers)
                    if (jailedPlayer.CurrentRound)
                        jailedPlayer.CurrentRound = false;

                if (!_pluginConfig.SaveOverwatchs && !_pluginConfig.SaveHiddenTags)
                    return;

                var overwatchCache = File.ReadAllLines(Plugin.OverwatchFilePath).ToList();

                var tagsCache = File.ReadAllLines(Plugin.OverwatchFilePath).ToList();

                foreach (var player in Player.List)
                {
                    var userId = player.UserId;

                    if (_pluginConfig.SaveOverwatchs)
                    {
                        if (player.IsOverwatchEnabled && !overwatchCache.Contains(userId))
                            overwatchCache.Add(userId);

                        else if (!player.IsOverwatchEnabled && overwatchCache.Contains(userId))
                            overwatchCache.Remove(userId);
                    }

                    if (!_pluginConfig.SaveHiddenTags) continue;

                    if (player.BadgeHidden && !tagsCache.Contains(userId))
                        tagsCache.Add(userId);

                    else if (!player.BadgeHidden && tagsCache.Contains(userId))
                        tagsCache.Remove(userId);
                }

                if (_pluginConfig.SaveOverwatchs)
                    File.WriteAllLines(Plugin.OverwatchFilePath, overwatchCache);

                if (_pluginConfig.SaveHiddenTags)
                    File.WriteAllLines(Plugin.OverwatchFilePath, overwatchCache);
            }
            catch (Exception e)
            {
                Log.Error($"Round End: {e}");
            }
        }

        internal void OnTriggerTesla(TriggeringTeslaEventArgs ev)
        {
            if (_pluginConfig.GodsIgnoreTeslas && ev.Player.IsGodModeEnabled)
                ev.IsAllowed = false;
        }

        internal void OnSetClass(ChangingRoleEventArgs ev)
        {
            if (_pluginConfig.GodTuts)
                ev.Player.IsGodModeEnabled = ev.NewRole == RoleTypeId.Tutorial;
        }

        internal void OnPlayerInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Player.HasSessionVariable("AT-BreakDoors"))
                ev.Door.BreakDoor();

            if (ev.Player.HasSessionVariable("AT-PryGates"))
                ev.Door.TryPryOpen();
        }

        internal void OnPlayerHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != ev.Player && ev.Attacker.HasSessionVariable("AT-InstantKill"))
                ev.Amount = int.MaxValue;
        }
    }
}