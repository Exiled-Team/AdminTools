using System;
using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using Handlers = Exiled.Events.Handlers;
using UnityEngine;
using MEC;

namespace AdminTools
{
	public class Plugin : Plugin<Config>
	{
		public override string Author { get; } = "Originally by Galaxy119. Modifications by KoukoCocoa & Thomasjosif";
		public override string Name { get; } = "Admin Tools";
		public override string Prefix { get; } = "AT";
		public override Version RequiredExiledVersion { get; } = new Version(3, 0, 0);

		public EventHandlers EventHandlers;
		public static System.Random NumGen = new System.Random();
		public static List<Jailed> JailedPlayers = new List<Jailed>();
		public static Dictionary<Player, InstantKillComponent> IkHubs = new Dictionary<Player, InstantKillComponent>();
		public static Dictionary<Player, BreakDoorComponent> BdHubs = new Dictionary<Player, BreakDoorComponent>();
		public static Dictionary<Player, RegenerationComponent> RgnHubs = new Dictionary<Player, RegenerationComponent>();
		public static HashSet<Player> PryGateHubs = new HashSet<Player>();
		public static Dictionary<Player, List<GameObject>> BchHubs = new Dictionary<Player, List<GameObject>>();
		public static Dictionary<Player, List<GameObject>> DumHubs = new Dictionary<Player, List<GameObject>>();
		public static float HealthGain = 5;
		public static float HealthInterval = 1;
		public string OverwatchFilePath;
		public string HiddenTagsFilePath;
		public static bool RestartOnEnd = false;
		public static HashSet<Player> RoundStartMutes = new HashSet<Player>();
		public static List<CoroutineHandle> JailCoroutine = new List<CoroutineHandle>();
		public override void OnEnabled()
		{
			try
			{
				string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string pluginPath = Path.Combine(appData, "Plugins");
				string path = Path.Combine(Paths.Plugins, "AdminTools");
				string overwatchFileName = Path.Combine(path, "AdminTools-Overwatch.txt");
				string hiddenTagFileName = Path.Combine(path, "AdminTools-HiddenTags.txt");

				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				if (!File.Exists(overwatchFileName))
					File.Create(overwatchFileName).Close();

				if (!File.Exists(hiddenTagFileName))
					File.Create(hiddenTagFileName).Close();

				OverwatchFilePath = overwatchFileName;
				HiddenTagsFilePath = hiddenTagFileName;

				EventHandlers = new EventHandlers(this);
				Handlers.Player.Verified += EventHandlers.OnPlayerVerified;
				Handlers.Server.RoundEnded += EventHandlers.OnRoundEnd;
				Handlers.Player.TriggeringTesla += EventHandlers.OnTriggerTesla;
				Handlers.Player.ChangingRole += EventHandlers.OnSetClass;
				Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
				Handlers.Player.InteractingDoor += EventHandlers.OnDoorOpen;
				Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
				Handlers.Player.Destroying += EventHandlers.OnPlayerDestroyed;
			}
			catch (Exception e)
			{
				Log.Error($"Loading error: {e}");
			}
		}

		public override void OnDisabled()
		{
			Handlers.Player.InteractingDoor -= EventHandlers.OnDoorOpen;
			Handlers.Player.Verified -= EventHandlers.OnPlayerVerified;
			Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnd;
			Handlers.Player.TriggeringTesla -= EventHandlers.OnTriggerTesla;
			Handlers.Player.ChangingRole -= EventHandlers.OnSetClass;
			Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
			Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
			Handlers.Player.Destroying -= EventHandlers.OnPlayerDestroyed;
			EventHandlers = null;
			NumGen = null;
		}

		public override void OnReloaded() { }
	}
}