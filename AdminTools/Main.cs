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
    /// <summary>
    /// Main plugin class for Admin Tools.
    /// </summary>
    public class Main : Plugin<Config>
    {
        /// <summary>
        /// List of players currently in Overwatch mode.
        /// </summary>
        public static List<string> Overwatch { get; internal set; }

        /// <summary>
        /// Dictionary of players who are jailed, keyed by their string identifier.
        /// </summary>
        public static Dictionary<string, Jailed> JailedPlayers { get; } = new();

        /// <summary>
        /// List of players near Pry Gate.
        /// </summary>
        public static List<Player> PryGate { get; } = new();

        /// <summary>
        /// List of players with instant kill capabilities.
        /// </summary>
        public static List<Player> InstantKill { get; } = new();

        /// <summary>
        /// List of players capable of breaking doors.
        /// </summary>
        public static List<Player> BreakDoors { get; } = new();

        /// <summary>
        /// List of players muted at round start.
        /// </summary>
        public static List<Player> RoundStartMutes { get; } = new();

        /// <summary>
        /// Dictionary mapping players to their related BCH hubs.
        /// </summary>
        public static Dictionary<Player, List<GameObject>> BchHubs { get; } = new();

        /// <summary>
        /// Gets the file path for Overwatch data.
        /// </summary>
        public string OverwatchFilePath { get; private set; }

        /// <summary>
        /// Gets the Harmony instance used for patching.
        /// </summary>
        public Harmony Harmony { get; } = new("Exiled-AdminTools");

        /// <summary>
        /// Gets or sets the event handlers.
        /// </summary>
        public EventHandlers EventHandlers { get; private set; }

        /// <inheritdoc/> 
        public override string Author { get; } = "Exiled-Team";

        /// <inheritdoc/> 
        public override string Name { get; } = "Admin Tools";

        /// <inheritdoc/> 
        public override string Prefix { get; } = "AdminTools";

        /// <inheritdoc/> 
        public override PluginPriority Priority { get; } = (PluginPriority)1;

        /// <inheritdoc/> 
        public override Version Version { get; } = new(8, 0, 0);

        /// <inheritdoc/> 
        public override Version RequiredExiledVersion { get; } = new(8, 8, 0);

        /// <inheritdoc/> 
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

        /// <inheritdoc/> 
        public override void OnDisabled()
        {
            Harmony?.UnpatchAll(Harmony.Id);
            EventHandlers = null;

            base.OnDisabled();
        }
    }
}