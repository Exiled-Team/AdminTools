namespace AdminTools
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    public class Config : IConfig
    {
        [Description("Should the tutorial class be in God Mode? Default: true")]
        public bool GodTuts { get; set; } = true;

        [Description("Should players in God mode not trigger the Tesla gates? Default: true")]
        public bool GodsIgnoreTeslas { get; set; } = true;

        [Description("Should overwatch mode be saved when reconnecting? Default: true")]
        public bool SaveOverwatchs { get; set; } = true;

        [Description("Should hidden tags be saved when reconnecting? Default: true")]
        public bool SaveHiddenTags { get; set; } = true;

        [Description("Enable/Disable AdminTools.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should the debug logs be shown. Default: false")]
        public bool Debug { get; set; }
    }
}