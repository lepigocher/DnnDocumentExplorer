using DotNetNuke.Entities.Modules;

using TidyModules.DocumentExplorer.Common;

namespace TidyModules.DocumentExplorer
{
    public sealed class DocumentSettings : SettingsWrapper
    {
        #region Contructor

        public DocumentSettings(ModuleInfo module) : base(module)
        {
        }

        #endregion

        #region Properties

        [ModuleSetting("JqueryUICSS", "http://code.jquery.com/ui/1.12.0/themes/base/jquery-ui.css")]
        public string JqueryUICSS { get; set; }

        [ModuleSetting("PrimeUIJS", "https://cdnjs.cloudflare.com/ajax/libs/primeui/4.1.14/primeui.min.js")]
        public string PrimeUIJS { get; set; }

        [ModuleSetting("PrimeUICSS", "https://cdnjs.cloudflare.com/ajax/libs/primeui/4.1.14/primeui.min.css")]
        public string PrimeUICSS { get; set; }

        [ModuleSetting("FontAwesomeCSS", "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.6.3/css/font-awesome.min.css")]
        public string FontAwesomeCSS { get; set; }

        [ModuleSetting("Theme","(none)")]
        public string Theme { get; set; }

        [ModuleSetting("Rows", "10")]
        public int Rows { get; set; }

        [ModuleSetting("ResetFilters", "true")]
        public bool ResetFilters { get; set; }

        [ModuleSetting("ShowIcon", "true")]
        public bool ShowIcon { get; set; }

        [ModuleSetting("NameHeaderClass", "defHeaderName")]
        public string NameHeaderClass { get; set; }

        [ModuleSetting("NameBodyClass", "defBodyName")]
        public string NameBodyClass { get; set; }

        [ModuleSetting("NameFilter", "true")]
        public bool NameFilter { get; set; }

        [ModuleSetting("NameFilterMatchMode", "contains")]
        public string NameFilterMatchMode { get; set; }

        [ModuleSetting("NameSortable", "true")]
        public bool NameSortable { get; set; }

        [ModuleSetting("ShowDate", "true")]
        public bool ShowDate { get; set; }

        [ModuleSetting("DateHeaderClass", "defHeaderDate")]
        public string DateHeaderClass { get; set; }

        [ModuleSetting("DateBodyClass", "defBodyDate")]
        public string DateBodyClass { get; set; }

        [ModuleSetting("DateFilter", "false")]
        public bool DateFilter { get; set; }

        [ModuleSetting("DateFilterMatchMode", "contains")]
        public string DateFilterMatchMode { get; set; }

        [ModuleSetting("DateSortable", "true")]
        public bool DateSortable { get; set; }

        [ModuleSetting("ShowSize", "true")]
        public bool ShowSize { get; set; }

        [ModuleSetting("SizeHeaderClass", "defHeaderSize")]
        public string SizeHeaderClass { get; set; }

        [ModuleSetting("SizeBodyClass", "defBodySize")]
        public string SizeBodyClass { get; set; }

        [ModuleSetting("SizeFilter", "false")]
        public bool SizeFilter { get; set; }

        [ModuleSetting("SizeFilterMatchMode", "contains")]
        public string SizeFilterMatchMode { get; set; }

        [ModuleSetting("SizeSortable", "true")]
        public bool SizeSortable { get; set; }

        [ModuleSetting("UserFolder", "true")]
        public bool UserFolder { get; set; }

        [ModuleSetting("FileManagement", "true")]
        public bool FileManagement { get; set; }

        [ModuleSetting("OpenOnDblclick", "false")]
        public bool OpenOnDblclick { get; set; }

        [ModuleSetting("ImagePreview", "true")]
        public bool ImagePreview { get; set; }

        [ModuleSetting("ThumbnailWidth", "450")]
        public int ThumbnailWidth { get; set; }

        [ModuleSetting("ThumbnailHeight", "300")]
        public int ThumbnailHeight { get; set; }

        #endregion
    }
}