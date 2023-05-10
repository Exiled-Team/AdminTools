using Exiled.API.Interfaces;
using Exiled.Loader;
using System.ComponentModel;

namespace AdminTools
{
    public class Config : IConfig
    {
        [Description("Enable/Disable AdminTools.")]
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; }

        [Description("Should the tutorial class be in God Mode? Default: true")]
        public bool GodTuts { get; set; } = true;
        
        [Description("Can the vanished players see each other? Default: true")]
        public bool VanishedSeeEachOther { get; set; } = true;
    }
}