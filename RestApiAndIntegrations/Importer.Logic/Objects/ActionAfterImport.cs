using System.ComponentModel;

namespace AD.CAAPS.Importer.Logic
{
    public enum ActionAfterImport
    {
        [Description("None")]
        None = 0,
        [Description("Delete")]
        Delete = 1,
        [Description("Move")]
        Move = 2
    }
}