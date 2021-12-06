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
        [Description("Should the tutorial class be in ignored by Tesla Gates? Default: true")]
        public bool IgnoreTuts { get; set; } = true;
        [Description("Enable/Disable Auto Overwatch.")]
        public bool AutoOverwatch { get; set; } = true;
        [Description("Enable/Disable Auto Hidetag.")]
        public bool AutoHidetag { get; set; } = true;
    }
}