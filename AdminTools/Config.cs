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
        
        [Description("Should overwatch mode be saved when reconnecting? Default: true")]
        public bool SavingOverwatch { get; set; } = true;
        
        [Description("Should hidden tags be saved when reconnecting? Default: true")]
        public bool SavingHiddenTags { get; set; } = true;
    }
}