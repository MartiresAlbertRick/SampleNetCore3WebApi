using System.ComponentModel;

namespace AD.CAAPS.Importer.Common
{
    public enum ClearTargetTableSetting
    {
        [Description("None")]
        None = 0,

        [Description("Truncate")]
        Truncate = 1,

        [Description("Delete")]
        Delete = 2
    }
}