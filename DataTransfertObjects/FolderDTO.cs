using System.Collections.Generic;

namespace TidyModules.DocumentExplorer.DataTransfertObjects
{
    public sealed class FolderDTO
    {
        public string Label { get; set; }
        public FolderDataDTO Data { get; set; }
        public bool Leaf { get; set; }
        public bool Expanded { get; set; }
        public IEnumerable<FolderDTO> Children { get; set; }
    }
}