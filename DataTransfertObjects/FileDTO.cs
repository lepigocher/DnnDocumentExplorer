using System;

namespace TidyModules.DocumentExplorer.DataTransfertObjects
{
    public sealed class FileDTO
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public DateTime Modified { get; set; }
        public int Size { get; set; }
    }
}