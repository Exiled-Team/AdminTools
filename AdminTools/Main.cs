using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdminTools.Patches;
using CommandSystem.Commands.RemoteAdmin.Doors;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;
using Utils;

namespace AdminTools
{
	public class Main : Plugin<Config>
	{
		public static List<string> Overwatch { get; internal set; }
		public static Dictionary<string, Jailed> JailedPlayers { get; } = new();
		public static List<Player> PryGate { get; } = new();
		public static List<Player> InstantKill { get; } = new();
		public static List<Player> BreakDoors { get; } = new();
		public static List<Player> RoundStartMutes { get; } = new();
		public static Dictionary<Player, List<GameObject>> BchHubs { get; } = new();

		public string OverwatchFilePath { get; private set; }

		public Harmony Harmony { get; } = new("Exiled-AdminTools");
		public EventHandlers EventHandlers { get; private set; }
		
		public override string Author { get; } = "Exiled-Team";
		public override string Name { get; } = "Admin Tools";
		public override string Prefix { get; } = "AdminTools";
		public override PluginPriority Priority { get; } = (PluginPriority)1;
		public override Version Version { get; } = new(8, 0, 0);
        public override Version RequiredExiledVersion { get; } = new(8, 8, 0);

        public override void OnEnabled()
		{
			try
			{
				string path = Path.Combine(Paths.Configs, "AdminTools");
				string overwatchFileName = Path.Combine(path, "AdminTools-Overwatch.txt");
                OverwatchFilePath = overwatchFileName;

                if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				if (!File.Exists(overwatchFileName))
					File.Create(overwatchFileName).Close();
				else
                    Overwatch = File.ReadAllLines(overwatchFileName).ToList();
            }
            catch (Exception e)
            {
                Log.Error($"Loading error: {e}");
            }

            EventHandlers = new(this);

            if (Config.ExtendedCommandUsage)
            {
	            Harmony.Patch(AccessTools.Method(typeof(RAUtils), nameof(RAUtils.ProcessPlayerIdOrNamesList)), new(AccessTools.Method(typeof(CustomRAUtilsAddon), nameof(CustomRAUtilsAddon.Prefix))));
	            Harmony.Patch(AccessTools.Method(typeof(BaseDoorCommand), nameof(BaseDoorCommand.Execute)), transpiler: new(AccessTools.Method(typeof(DoorCommandPatch), nameof(DoorCommandPatch.Transpiler))));
            }

            base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Harmony?.UnpatchAll(Harmony.Id);
            EventHandlers = null;
            
            base.OnDisabled();
		}
	}
}