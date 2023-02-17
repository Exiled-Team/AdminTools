using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Permissions;
using Interactables.Interobjects;
using MEC;
using Mirror;
using NorthwoodLib.Pools;
using RemoteAdmin;
using UnityEngine;
using Log = Exiled.API.Features.Log;
using Object = UnityEngine.Object;

namespace AdminTools
{
	using Exiled.API.Extensions;
	using Exiled.API.Features.Items;
	using Exiled.Events.EventArgs.Player;
	using Exiled.Events.EventArgs.Server;
	using Footprinting;
	using InventorySystem.Items.Firearms.Attachments;
	using InventorySystem.Items.Pickups;
	using InventorySystem.Items.ThrowableProjectiles;
	using PlayerRoles;
	using PlayerStatsSystem;
	using Ragdoll = Exiled.API.Features.Ragdoll;

	public class EventHandlers
	{
		private readonly Plugin _plugin;
		public EventHandlers(Plugin plugin) => this._plugin = plugin;
		public static List<Player> BreakDoorsList { get; } = new();

		public void OnDoorOpen(InteractingDoorEventArgs ev)
		{
			if (Plugin.PryGateHubs.Contains(ev.Player))
				ev.Door.TryPryOpen();
		}

		public static string FormatArguments(ArraySegment<string> sentence, int index)
		{
			StringBuilder sb = new();
			foreach (string word in sentence.Segment(index))
			{
				sb.Append(word);
				sb.Append(" ");
			}
			string msg = sb.ToString();
			return msg;
		}

		public static IEnumerator<float> SpawnBodies(Player player, RoleTypeId role, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Ragdoll.CreateAndSpawn(role, "SCP-343", "End of the Universe", player.Position, default, null);
				yield return Timing.WaitForSeconds(0.15f);
			}
		}

        public void OnPlayerDestroyed(DestroyingEventArgs ev)
        {
			if (Plugin.RoundStartMutes.Contains(ev.Player))
            {
				ev.Player.IsMuted = false;
				Plugin.RoundStartMutes.Remove(ev.Player);
            }
        }

        public static void SpawnWorkbench(Player ply, Vector3 position, Vector3 rotation, Vector3 size, out int benchIndex)
		{
			try
			{
				Log.Debug($"Spawning workbench");
				benchIndex = 0;
				GameObject bench =
					Object.Instantiate(
						NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Work Station"));
				rotation.x += 180;
				rotation.z += 180;
				Offset offset = new();
				offset.position = position;
				offset.rotation = rotation;
				offset.scale = Vector3.one;
				bench.gameObject.transform.localScale = size;
				NetworkServer.Spawn(bench);
				if (Plugin.BchHubs.TryGetValue(ply, out List<GameObject> objs))
				{
					objs.Add(bench);
				}
				else
				{
					Plugin.BchHubs.Add(ply, new List<GameObject>());
					Plugin.BchHubs[ply].Add(bench);
					benchIndex = Plugin.BchHubs[ply].Count();
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

        public static void SetPlayerScale(Player target, float x, float y, float z)
		{
			try
			{
				target.Scale = new Vector3(x, y, z);
			}
			catch (Exception e)
			{
				Log.Info($"Set Scale error: {e}");
			}
		}

		public static void SetPlayerScale(Player target, float scale)
		{
			try
			{
				target.Scale = Vector3.one * scale;
			}
			catch (Exception e)
			{
				Log.Info($"Set Scale error: {e}");
			}
		}

		public static IEnumerator<float> DoRocket(Player player, float speed)
		{
			const int maxAmnt = 50;
			int amnt = 0;
			while (player.Role != RoleTypeId.Spectator)
			{
				player.Position += Vector3.up * speed;
				amnt++;
				if (amnt >= maxAmnt)
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

		public static IEnumerator<float> DoJail(Player player, bool skipadd = false)
		{
			List<Item> items = new();
			Dictionary<AmmoType, ushort> ammo = new();
			foreach (KeyValuePair<ItemType, ushort> kvp in player.Ammo)
				ammo.Add(kvp.Key.GetAmmoType(), kvp.Value);
			foreach (Item item in player.Items)
				items.Add(item);
			if (!skipadd)
			{
				Plugin.JailedPlayers.Add(new Jailed
				{
					Health = player.Health,
					Position = player.Position,
					Items = items,
					Name = player.Nickname,
					Role = player.Role,
					Userid = player.UserId,
					CurrentRound = true,
					Ammo = ammo
				});
			}

			if (player.IsOverwatchEnabled)
				player.IsOverwatchEnabled = false;
			yield return Timing.WaitForSeconds(1f);
			player.ClearInventory(false);
			player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);
			player.Position = new Vector3(40f, 1015f, -33f);
		}

		public static IEnumerator<float> DoUnJail(Player player)
		{
			Jailed jail = Plugin.JailedPlayers.Find(j => j.Userid == player.UserId);
			if (jail.CurrentRound)
			{
				player.Role.Set(jail.Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
				yield return Timing.WaitForSeconds(0.5f);
				try
				{
					player.ResetInventory(jail.Items);
					player.Health = jail.Health;
					player.Position = jail.Position;
					foreach (KeyValuePair<AmmoType, ushort> kvp in jail.Ammo)
						player.Ammo[kvp.Key.GetItemType()] = kvp.Value;
				}
				catch (Exception e)
				{
					Log.Error($"{nameof(DoUnJail)}: {e}");
				}
			}
			else
			{
				player.Role.Set(RoleTypeId.Spectator);
			}
			Plugin.JailedPlayers.Remove(jail);
		}

		public void OnPlayerVerified(VerifiedEventArgs ev)
		{
			try
			{
				if (Plugin.JailedPlayers.Any(j => j.Userid == ev.Player.UserId))
					Timing.RunCoroutine(DoJail(ev.Player, true));

				if (File.ReadAllText(_plugin.OverwatchFilePath).Contains(ev.Player.UserId))
				{
					Log.Debug($"Putting {ev.Player.UserId} into overwatch.");
					Timing.CallDelayed(1, () => ev.Player.IsOverwatchEnabled = true);
				}

				if (File.ReadAllText(_plugin.HiddenTagsFilePath).Contains(ev.Player.UserId))
				{
					Log.Debug($"Hiding {ev.Player.UserId}'s tag.");
					Timing.CallDelayed(1, () => ev.Player.BadgeHidden = true);
				}

				if (Plugin.RoundStartMutes.Count != 0 && !ev.Player.ReferenceHub.serverRoles.RemoteAdmin && !Plugin.RoundStartMutes.Contains(ev.Player))
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

		public void OnRoundStart()
		{
			foreach (Player ply in Plugin.RoundStartMutes)
			{
				if (ply != null)
				{
					ply.IsMuted = false;
				}
			}
			Plugin.RoundStartMutes.Clear();
		}

		public void OnRoundEnd(RoundEndedEventArgs ev)
		{
			try
			{
				List<string> overwatchRead = File.ReadAllLines(_plugin.OverwatchFilePath).ToList();
				List<string> tagsRead = File.ReadAllLines(_plugin.HiddenTagsFilePath).ToList();

				foreach (Player player in Player.List)
				{
					string userId = player.UserId;

					if (player.IsOverwatchEnabled && !overwatchRead.Contains(userId))
						overwatchRead.Add(userId);
					else if (!player.IsOverwatchEnabled && overwatchRead.Contains(userId))
						overwatchRead.Remove(userId);

					if (player.BadgeHidden && !tagsRead.Contains(userId))
						tagsRead.Add(userId);
					else if (!player.BadgeHidden && tagsRead.Contains(userId))
						tagsRead.Remove(userId);
				}

				foreach (string s in overwatchRead)
					Log.Debug($"{s} is in overwatch.");
				foreach (string s in tagsRead)
					Log.Debug($"{s} has their tag hidden.");
				File.WriteAllLines(_plugin.OverwatchFilePath, overwatchRead);
				File.WriteAllLines(_plugin.HiddenTagsFilePath, tagsRead);

				// Update all the jails that it is no longer the current round, so when they are unjailed they don't teleport into the void.
				foreach (Jailed jail in Plugin.JailedPlayers)
				{
					if(jail.CurrentRound)
						jail.CurrentRound = false;
				}
			}
			catch (Exception e)
			{
				Log.Error($"Round End: {e}");
			}

			if (Plugin.RestartOnEnd)
			{
				Log.Info("Restarting server....");
				Round.Restart(false, true, ServerStatic.NextRoundAction.Restart);
			}
		}

		public void OnTriggerTesla(TriggeringTeslaEventArgs ev)
		{
			if (ev.Player.IsGodModeEnabled)
				ev.IsAllowed = false;
		}

		public void OnSetClass(ChangingRoleEventArgs ev)
		{
			if (_plugin.Config.GodTuts)
				ev.Player.IsGodModeEnabled = ev.NewRole == RoleTypeId.Tutorial;
		}

		public void OnWaitingForPlayers()
		{
			Plugin.IkHubs.Clear();
			BreakDoorsList.Clear();
		}

		public void OnPlayerInteractingDoor(InteractingDoorEventArgs ev)
		{
			if (BreakDoorsList.Contains(ev.Player))
				ev.Door.BreakDoor();
		}
	}
}
