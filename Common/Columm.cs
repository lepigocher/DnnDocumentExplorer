namespace TidyModules.DocumentExplorer.Common
{
    public sealed class Column
    {
        public string Field { get; set; }
        public string HeaderText { get; set; }
        //public string FooterText { get; set; }
        public bool Sortable { get; set; }
        //public string HeaderStyle { get; set; }
        public string HeaderClass { get; set; }
        //public string BodyStyle { get; set; }
        public string BodyClass { get; set; }
        //public int Colspan { get; set; }
        //public int Rowspan { get; set; }
        public bool Filter { get; set; }
        public string FilterMatchMode { get; set; }
        //public string FilterFunction { get; set; }
        public string Editor { get; set; }
        //public bool RowToggler { get; set; }
    }
}