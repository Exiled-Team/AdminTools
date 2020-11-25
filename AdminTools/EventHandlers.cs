using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Permissions;
using Grenades;
using MEC;
using Mirror;
using NorthwoodLib.Pools;
using RemoteAdmin;
using UnityEngine;
using Log = Exiled.API.Features.Log;
using Object = UnityEngine.Object;

namespace AdminTools
{
	public class EventHandlers
	{
		private readonly Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin;

		public static void LogCommandUsed(CommandSender sender, string Command)
		{
			string data =
				$"{DateTime.Now}: {sender.Nickname} ({sender.SenderId}) executed: {Command} {Environment.NewLine}";
			File.AppendAllText(Paths.Log, data);
		}

		public void OnCommand(SendingRemoteAdminCommandEventArgs ev)
		{
			if (ev.Name.Contains("REQUEST_DATA PLAYER_LIST"))
				return;
		}

		public void OnDoorOpen(InteractingDoorEventArgs ev)
		{
			if (Plugin.PryGateHubs.Contains(ev.Player))
				ev.Door.PryGate();
		}

		public static string FormatArguments(ArraySegment<string> sentence, int index)
		{
			StringBuilder SB = StringBuilderPool.Shared.Rent();
			foreach (string word in sentence.Segment(index))
			{
				SB.Append(word);
				SB.Append(" ");
			}
			string msg = SB.ToString();
			StringBuilderPool.Shared.Return(SB);
			return msg;
		}

		public static void SpawnDummyModel(Player Ply, Vector3 position, Quaternion rotation, RoleType role, float x, float y, float z, out int DummyIndex)
		{
			DummyIndex = 0;
			GameObject obj =
				Object.Instantiate(
					NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));
			CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();
			if (ccm == null)
				Log.Error("CCM is null, this can cause problems!");
			ccm.CurClass = role;
			ccm.GodMode = true;
			ccm.OldRefreshPlyModel(PlayerManager.localPlayer);
			obj.GetComponent<NicknameSync>().Network_myNickSync = "Dummy";
			obj.GetComponent<QueryProcessor>().PlayerId = 9999;
			obj.GetComponent<QueryProcessor>().NetworkPlayerId = 9999;
			obj.transform.localScale = new Vector3(x, y, z);
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			NetworkServer.Spawn(obj);
			if (Plugin.DumHubs.TryGetValue(Ply, out List<GameObject> objs))
			{
				objs.Add(obj);
			}
			else
			{
				Plugin.DumHubs.Add(Ply, new List<GameObject>());
				Plugin.DumHubs[Ply].Add(obj);
				DummyIndex = Plugin.DumHubs[Ply].Count();
			}
			if (DummyIndex != 1)
				DummyIndex = objs.Count();
		}

		public static IEnumerator<float> SpawnBodies(Player player, RoleType role, int count)
		{
			for (int i = 0; i < count; i++)
			{
				player.GameObject.GetComponent<RagdollManager>().SpawnRagdoll(player.Position + Vector3.up * 5,
					Quaternion.identity, Vector3.zero, (int)role,
					new PlayerStats.HitInfo(1000f, player.UserId, DamageTypes.Falldown,
						player.Id), false, "SCP-343", "SCP-343", 0);
				yield return Timing.WaitForSeconds(0.15f);
			}
		}

		public static void SpawnWorkbench(Player Ply, Vector3 position, Vector3 rotation, Vector3 size, out int BenchIndex)
		{
			BenchIndex = 0;
			GameObject bench =
				Object.Instantiate(
					NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Work Station"));
			rotation.x += 180;
			rotation.z += 180;
			Offset offset = new Offset();
			offset.position = position;
			offset.rotation = rotation;
			offset.scale = Vector3.one;
			bench.gameObject.transform.localScale = size;
			NetworkServer.Spawn(bench);
			if (Plugin.BchHubs.TryGetValue(Ply, out List<GameObject> objs))
			{
				objs.Add(bench);
			}
			else
			{
				Plugin.BchHubs.Add(Ply, new List<GameObject>());
				Plugin.BchHubs[Ply].Add(bench);
				BenchIndex = Plugin.BchHubs[Ply].Count();
			}
			if (BenchIndex != 1)
				BenchIndex = objs.Count();
			bench.GetComponent<WorkStation>().Networkposition = offset;
			bench.AddComponent<WorkStation>();
		}

		public static void SpawnItem(ItemType type, Vector3 pos, Vector3 rot)
		{
			Exiled.API.Extensions.Item.Spawn(type, 0, pos);
		}

		public static void SetPlayerScale(GameObject target, float x, float y, float z)
		{
			try
			{
				NetworkIdentity identity = target.GetComponent<NetworkIdentity>();
				target.transform.localScale = new Vector3(1 * x, 1 * y, 1 * z);

				ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage();
				destroyMessage.netId = identity.netId;

				foreach (GameObject player in PlayerManager.players)
				{
					NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;
					if (player != target)
						playerCon.Send(destroyMessage, 0);

					object[] parameters = new object[] { identity, playerCon };
					typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
				}
			}
			catch (Exception e)
			{
				Log.Info($"Set Scale error: {e}");
			}
		}

		public static void SetPlayerScale(GameObject target, float scale)
		{
			try
			{
				NetworkIdentity identity = target.GetComponent<NetworkIdentity>();
				target.transform.localScale = Vector3.one * scale;

				ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage();
				destroyMessage.netId = identity.netId;

				foreach (GameObject player in PlayerManager.players)
				{
					if (player == target)
						continue;

					NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;
					playerCon.Send(destroyMessage, 0);

					object[] parameters = new object[] { identity, playerCon };
					typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
				}
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
			while (player.Role != RoleType.Spectator)
			{
				player.Position += Vector3.up * speed;
				amnt++;
				if (amnt >= maxAmnt)
				{
					player.IsGodModeEnabled = false;
					SpawnGrenadeOnPlayer(player, GrenadeType.Frag, 0.05f);
					player.Hurt(-1, null, "ADMIN");
				}

				yield return Timing.WaitForOneFrame;
			}
		}

		// I found the below code to be very overcomplicated for the task at hand. Not sure why it was written like this......

		/*
		public static IEnumerator<float> DoTut(Player player)
		{
			if (player.IsOverwatchEnabled)
				player.IsOverwatchEnabled = false;

			player.Role = RoleType.Tutorial;
			yield return Timing.WaitForSeconds(1f);
			Door[] d = UnityEngine.Object.FindObjectsOfType<Door>();
			foreach (Door door in d)
				if (door.DoorName == "SURFACE_GATE")
				{
					player.Position = door.transform.position + Vector3.up * 2;
					break;
				}

			player.ReferenceHub.serverRoles.CallTargetSetNoclipReady(player.ReferenceHub.characterClassManager.connectionToClient, true);
			player.ReferenceHub.serverRoles.NoclipReady = true;
		}*/

		public static IEnumerator<float> DoJail(Player player, bool skipadd = false)
		{
			List<Inventory.SyncItemInfo> items = new List<Inventory.SyncItemInfo>();
			Dictionary<AmmoType, uint> ammo = new Dictionary<AmmoType, uint>();
			foreach(AmmoType type in (AmmoType []) Enum.GetValues(typeof(AmmoType)))
			{
				ammo[type] = player.Ammo[(int)type];
			}
			foreach (Inventory.SyncItemInfo item in player.Inventory.items)
				items.Add(item);
			if (!skipadd)
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
			if (player.IsOverwatchEnabled)
				player.IsOverwatchEnabled = false;
			yield return Timing.WaitForSeconds(1f);
			player.Role = RoleType.Tutorial;
			player.Position = new Vector3(53f, 1020f, -44f);
			player.Inventory.items.Clear();
		}

		public static IEnumerator<float> DoUnJail(Player player)
		{
			Jailed jail = Plugin.JailedPlayers.Find(j => j.Userid == player.UserId);
			if (jail.CurrentRound)
			{
				player.Role = jail.Role;
				player.ResetInventory(jail.Items);
				yield return Timing.WaitForSeconds(1.5f);
				player.Health = jail.Health;
				player.Position = jail.Position;
				foreach (AmmoType type in (AmmoType[])Enum.GetValues(typeof(AmmoType)))
				{
					player.Ammo[(int)type] = jail.Ammo[type];
				}
			}
			else
			{
				player.Role = RoleType.Spectator;
			}
			Plugin.JailedPlayers.Remove(jail);
		}

		public static void SpawnGrenadeOnPlayer(Player Player, GrenadeType Type, float Timer)
		{
			Vector3 spawnrand = new Vector3(UnityEngine.Random.Range(0f, 2f), UnityEngine.Random.Range(0f, 2f), UnityEngine.Random.Range(0f, 2f));
			GrenadeManager gm = Player.ReferenceHub.gameObject.GetComponent<GrenadeManager>();
			GrenadeSettings gs = null;
			switch (Type)
			{
				case GrenadeType.Frag:
					gs = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFrag);
					break;
				case GrenadeType.Flash:
					gs = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFlash);
					break;
				case GrenadeType.Scp018:
					gs = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.SCP018);
					break;
			}
			Grenade grenade = Object.Instantiate(gs.grenadeInstance).GetComponent<Grenade>();
			if (Type != GrenadeType.Scp018)
			{
				grenade.fuseDuration = Timer;
				grenade.FullInitData(gm, Player.Position, Quaternion.Euler(grenade.throwStartAngle), grenade.throwLinearVelocityOffset, grenade.throwAngularVelocity, Player.Team);
			}
			else
				grenade.InitData(gm, spawnrand, Vector3.zero);
			NetworkServer.Spawn(grenade.gameObject);
		}

		public static void SpawnBallOnPlayer(Player Player)
		{
			Vector3 spawnrand = new Vector3(UnityEngine.Random.Range(0f, 2f), UnityEngine.Random.Range(0f, 2f), UnityEngine.Random.Range(0f, 2f));
			GrenadeManager gm = Player.ReferenceHub.GetComponent<GrenadeManager>();
			GrenadeSettings ball = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.SCP018);
			Grenade component = Object.Instantiate(ball.grenadeInstance).GetComponent<Scp018Grenade>();
			component.InitData(gm, spawnrand, Vector3.zero);
			NetworkServer.Spawn(component.gameObject);
		}

		public void OnPlayerJoin(JoinedEventArgs ev)
		{
			try
			{
				if (Plugin.JailedPlayers.Any(j => j.Userid == ev.Player.UserId))
					Timing.RunCoroutine(DoJail(ev.Player, true));

				if (File.ReadAllText(plugin.OverwatchFilePath).Contains(ev.Player.UserId))
				{
					Log.Debug($"Putting {ev.Player.UserId} into overwatch.");
					ev.Player.IsOverwatchEnabled = true;
				}

				if (File.ReadAllText(plugin.HiddenTagsFilePath).Contains(ev.Player.UserId))
				{
					Log.Debug($"Hiding {ev.Player.UserId}'s tag.");
					ev.Player.BadgeHidden = true;
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
				List<string> overwatchRead = File.ReadAllLines(plugin.OverwatchFilePath).ToList();
				List<string> tagsRead = File.ReadAllLines(plugin.HiddenTagsFilePath).ToList();

				foreach (Player player in Player.List)
				{
					string userId = player.UserId;

					if (player.IsOverwatchEnabled && !overwatchRead.Contains(userId))
						overwatchRead.Add(userId);
					else if (!player.IsOverwatchEnabled && overwatchRead.Contains(userId))
						overwatchRead.Remove(userId);

					if (player.ReferenceHub.serverRoles._hideLocalBadge && !tagsRead.Contains(userId))
						tagsRead.Add(userId);
					else if (!player.ReferenceHub.serverRoles._hideLocalBadge && tagsRead.Contains(userId))
						tagsRead.Remove(userId);
				}

				foreach (string s in overwatchRead)
					Log.Debug($"{s} is in overwatch.");
				foreach (string s in tagsRead)
					Log.Debug($"{s} has their tag hidden.");
				File.WriteAllLines(plugin.OverwatchFilePath, overwatchRead);
				File.WriteAllLines(plugin.HiddenTagsFilePath, tagsRead);

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
				Round.Restart();

				Timing.CallDelayed(1.5f, Application.Quit);
			}
		}

		public void OnTriggerTesla(TriggeringTeslaEventArgs ev)
		{
			if (ev.Player.IsGodModeEnabled)
				ev.IsTriggerable = false;
		}

		public void OnSetClass(ChangingRoleEventArgs ev)
		{
			if (plugin.Config.GodTuts)
				ev.Player.IsGodModeEnabled = ev.NewRole == RoleType.Tutorial;
		}

		public void OnWaitingForPlayers()
		{
			Plugin.IkHubs.Clear();
			Plugin.BdHubs.Clear();
		}
	}
}
