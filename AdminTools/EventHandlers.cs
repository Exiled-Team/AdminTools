using System.IO;
using AdminTools.Commands;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Interfaces;
using Exiled.CustomModules;
using Exiled.CustomModules.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using PlayerStatsSystem;

namespace AdminTools
{
	using Log = Log;
	using Handlers = Exiled.Events.Handlers;

	public class EventHandlers
	{
		private readonly Main plugin;

		internal EventHandlers(Main main)
		{
			plugin = main;
			
			Handlers.Player.Verified += OnPlayerVerified;
			Handlers.Server.RoundEnded += OnRoundEnded;
			Handlers.Player.TriggeringTesla += OnTriggeringTesla;
			Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
			Handlers.Player.InteractingDoor += OnInteractingDoor;
			Handlers.Server.RoundStarted += OnRoundStarted;
			Handlers.Player.Destroying += OnPlayerDestroying;
			Handlers.Player.InteractingDoor += OnPlayerInteractingDoor;
			Handlers.Player.ChangingRole += OnChangingRole;
		}

		~EventHandlers()
		{
			Handlers.Player.Verified -= OnPlayerVerified;
			Handlers.Server.RoundEnded -= OnRoundEnded;
			Handlers.Player.TriggeringTesla -= OnTriggeringTesla;
			Handlers.Player.ChangingRole -= OnChangingRole;
			Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
			Handlers.Player.InteractingDoor -= OnInteractingDoor;
			Handlers.Server.RoundStarted -= OnRoundStarted;
			Handlers.Player.Destroying -= OnPlayerDestroying;
			Handlers.Player.InteractingDoor -= OnPlayerInteractingDoor;
			Handlers.Player.ChangingRole -= OnChangingRole;
		}

		internal void OnInteractingDoor(InteractingDoorEventArgs ev)
		{
			if (Main.PryGate.Contains(ev.Player) && ev.Door is Gate gate)
                gate.TryPry();
		}

		internal static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != ev.Player && Main.InstantKill.Contains(ev.Attacker))
                ev.Amount = StandardDamageHandler.KillValue;
        }

		internal void OnPlayerDestroying(DestroyingEventArgs ev)
        {
			if (Main.RoundStartMutes.Remove(ev.Player))
            {
				ev.Player.IsMuted = false;
            }
			if (!Round.IsEnded)
				ev.Player.SavingPlayerData();
        }

		internal void OnPlayerVerified(VerifiedEventArgs ev)
		{
            if (Main.JailedPlayers.ContainsKey(ev.Player.UserId))
                Jail.DoJail(ev.Player, true);

            if (ev.Player.RemoteAdminPermissions.HasFlag(PlayerPermissions.Overwatch) && Main.Overwatch.Contains(ev.Player.UserId))
            {
                Log.Debug($"Putting {ev.Player.UserId} into overwatch.");
                ev.Player.IsOverwatchEnabled = true;
            }

            if (Main.RoundStartMutes.Count != 0 && !ev.Player.RemoteAdminAccess && !Main.RoundStartMutes.Contains(ev.Player))
            {
                Log.Debug($"Muting {ev.Player.UserId} (no RA).");
                ev.Player.IsMuted = true;
                Main.RoundStartMutes.Add(ev.Player);
            }
        }

		internal void OnRoundStarted()
		{
			foreach (Player ply in Main.RoundStartMutes)
			{
				if (ply != null)
				{
					ply.IsMuted = false;
				}
			}
			Main.RoundStartMutes.Clear();
		}

		internal void OnRoundEnded(RoundEndedEventArgs ev)
        {
			// Update all the jails that it is no longer the current round, so when they are unjailed they don't teleport into the void.
            foreach (Jailed jail in Main.JailedPlayers.Values)
            {
                if (jail.CurrentRound)
                    jail.CurrentRound = false;
            }

            foreach (Player player in Player.List)
                player.SavingPlayerData();

            File.WriteAllLines(plugin.OverwatchFilePath, Main.Overwatch);
        }

		internal void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
		{
			if (ev.Player.IsGodModeEnabled)
				ev.IsAllowed = false;
		}

		internal void OnChangingRole(ChangingRoleEventArgs ev)
		{
			if (plugin.Config.GodTuts && (ev.Reason == SpawnReason.ForceClass || ev.Reason == SpawnReason.None))
				ev.Player.IsGodModeEnabled = ev.NewRole == RoleTypeId.Tutorial && !ev.Player.Is(out Pawn _);
		}

		internal void OnWaitingForPlayers()
		{
			Main.InstantKill.Clear();
            Main.BreakDoors.Clear();
            Main.PryGate.Clear();

            if (plugin.Config.ClearJailsOnRestart)
                Main.JailedPlayers.Clear();

            if (plugin.Config.DisableLockOnWaiting)
            {
	            Round.IsLobbyLocked = false;
	            Round.IsLocked = false;
            }
        }

		internal void OnPlayerInteractingDoor(InteractingDoorEventArgs ev)
		{
			if (Main.BreakDoors.Contains(ev.Player) && ev.Door is IDamageableDoor damageableDoor)
                damageableDoor.Break();
        }
	}
}
