using projet_easy_save_v2.src.models;
using System;

namespace prosoft_easysave.src.models
{
    struct StatusFileSaves
    {
        public DateTime Timestamp { get; set; }
        public string BackupConfigName { get; set; }
        public string BackupState { get; set; }
        public long TotalFiles { get; set; }
        public long FilesTotalSize { get; set; }
        public long FilesAlreadySaved { get; set; }
        public long TotalSizeNotSavedYetFiles { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
    }
}
