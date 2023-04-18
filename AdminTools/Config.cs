using Exiled.API.Interfaces;
using System.ComponentModel;

namespace AdminTools
{
    public class Config : IConfig
    {
        [Description("Enable/Disable AdminTools.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should the debug logs be shown. Default: false")]
        public bool Debug { get; set; }

        [Description("Should the tutorial class be in God Mode? Default: true")]
        public bool GodTuts { get; set; } = true;

        [Description("Should players in God mode not trigger the Tesla gates? Default: true")]
        public bool GodsIgnoreTeslas { get; set; } = true;
        
        [Description("Should overwatch mode be saved when reconnecting? Default: true")]
        public bool SaveOverwatchs { get; set; } = true;
        
        [Description("Should hidden tags be saved when reconnecting? Default: true")]
        public bool SaveHiddenTags { get; set; } = true;
    }
}