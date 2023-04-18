using System;
using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using Handlers = Exiled.Events.Handlers;
using UnityEngine;

namespace AdminTools
{
	public class Plugin : Plugin<Config>
	{
        private static readonly string CacheFilesDirectory = Path.Combine(Paths.Plugins, "AdminTools");
		internal static readonly string OverwatchFilePath = Path.Combine(CacheFilesDirectory, "AdminTools-Overwatch.txt");
        internal static readonly string HiddenTagsFilePath = Path.Combine(CacheFilesDirectory, "AdminTools-HiddenTags.txt");
        
        private EventHandlers _eventHandlers;
        
		public static System.Random NumGen = new();
		public static Dictionary<Player, List<GameObject>> SpawnedBenchHubs = new();
		public static readonly HashSet<Player> RoundStartMutes = new();
        
		// public static readonly Dictionary<Player, InstantKillComponent> InstantKillingHubs = new();
		// 
		// public static readonly HashSet<Player> PryGateHubs = new();

        public override string Author => "Originally by Joker119. Modifications by KoukoCocoa & Thomasjosif";
        public override string Name => "Admin Tools";
        public override string Prefix => "AT";
        
        public override Version RequiredExiledVersion { get; } = new(6, 0, 0);

        public override void OnEnabled()
		{
			try
			{
                if (!Directory.Exists(CacheFilesDirectory))
					Directory.CreateDirectory(CacheFilesDirectory);

				if (!File.Exists(OverwatchFilePath))
					File.Create(OverwatchFilePath).Close();

				if (!File.Exists(HiddenTagsFilePath))
					File.Create(HiddenTagsFilePath).Close();

                _eventHandlers = new EventHandlers(Config);
                
				Handlers.Player.Verified += _eventHandlers.OnPlayerVerified;
                Handlers.Player.Hurting += _eventHandlers.OnPlayerHurting;
				Handlers.Server.RoundEnded += _eventHandlers.OnRoundEnd;
				Handlers.Player.TriggeringTesla += _eventHandlers.OnTriggerTesla;
				Handlers.Player.ChangingRole += _eventHandlers.OnSetClass;
				Handlers.Server.RoundStarted += _eventHandlers.OnRoundStart;
				Handlers.Player.Destroying += _eventHandlers.OnPlayerDestroyed;
				Handlers.Player.InteractingDoor += _eventHandlers.OnPlayerInteractingDoor;
			}
			catch (Exception e)
			{
				Log.Error($"Loading error: {e}");
			}
		}

		public override void OnDisabled()
		{
            Handlers.Player.Verified -= _eventHandlers.OnPlayerVerified;
            Handlers.Server.RoundEnded -= _eventHandlers.OnRoundEnd;
            Handlers.Player.TriggeringTesla -= _eventHandlers.OnTriggerTesla;
            Handlers.Player.ChangingRole -= _eventHandlers.OnSetClass;
            Handlers.Server.RoundStarted -= _eventHandlers.OnRoundStart;
            Handlers.Player.Destroying -= _eventHandlers.OnPlayerDestroyed;
            Handlers.Player.InteractingDoor -= _eventHandlers.OnPlayerInteractingDoor;
            
			_eventHandlers = null;
			NumGen = null;
		}
    }
}