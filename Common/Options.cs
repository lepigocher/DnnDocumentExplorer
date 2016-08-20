using System.Collections.Generic;

namespace TidyModules.DocumentExplorer.Common
{
    public sealed class Options
    {
        public Dictionary<string, string> Resources { get; set; }
        public int Rows { get; set; }
        public bool ResetFilters { get; set; }
        public bool ShowIcon { get; set; }
        public List<Column> Columns { get; set; }
        public bool SynchronizeFolder { get; set; }
        public bool FileManagement { get; set; }
        public bool OpenOnDblclick { get; set; }
        public bool ImagePreview { get; set; }
    }
}