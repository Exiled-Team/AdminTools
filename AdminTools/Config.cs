using Exiled.API.Interfaces;
using Exiled.Loader;
using System.ComponentModel;

namespace AdminTools
{
    public class Config : IConfig
    {
        [Description("Enable/Disable AdminTools.")]
        public bool IsEnabled { get; set; } = true;
        [Description("Should the tutorial class be in God Mode? Default: true")]
        public bool GodTuts { get; set; } = true;
        [Description("Unjail all jailed players automatically when the round restarts? Default: false")]
        public bool ClearJailsOnRestart { get; set; } = false;
    }
}