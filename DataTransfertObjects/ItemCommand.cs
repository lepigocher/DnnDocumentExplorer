using System.Collections.Generic;

namespace TidyModules.DocumentExplorer.DataTransfertObjects
{
    /// <summary>
    /// Used to transmit item information to the command.
    /// </summary>
    public sealed class ItemCommand
    {
        /// <summary>
        /// Path of the selected folder
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// List of files
        /// </summary>
        public List<string> Files { get; set; }
        /// <summary>
        /// Boolean flag common to several commands
        /// </summary>
        public bool Flag { get; set; }
        /// <summary>
        /// Path of the destination folder
        /// </summary>
        public string ToPath { get; set; }
    }
}