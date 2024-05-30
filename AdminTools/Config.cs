using Exiled.API.Interfaces;
using System.ComponentModel;

namespace AdminTools
{
    public class Config : IConfig
    {
        [Description("Enable/Disable AdminTools.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not to show logs used for debugging.")]
        public bool Debug { get; set; } = false;

        [Description("Should Staff be in God Mode when they forceclass to Tutorial?? Default: false")]
        public bool GodTuts { get; set; } = false;

        [Description("Extending Command use for Getting a player (such as candy command and other parts of AdminTools). View the README on github for more info.")]
        public bool ExtendedCommandUsage { get; set; } = true;

        [Description("Unjail all jailed players automatically when the round restarts? Default: false")]
        public bool ClearJailsOnRestart { get; set; } = false;

        [Description("Whether or not to disable RoundLock & LobbyLock when Waiting For Players")]
        public bool DisableLockOnWaiting { get; set; } = false;
    }
}