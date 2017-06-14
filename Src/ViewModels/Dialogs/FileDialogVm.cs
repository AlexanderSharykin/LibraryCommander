﻿namespace ViewModels.Dialogs
{
    public class FileDialogVm
    {
        /// <summary>
        /// Gets or sets dialog title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or setw file types filter
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets select file name
        /// </summary>
        public string FileName { get; set; }
    }
}
