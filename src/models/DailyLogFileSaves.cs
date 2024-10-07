using System;

namespace prosoft_easysave.src.models
{

    struct DailyLogFileSaves
    {
        public DateTime Timestamp { get; set; }
        public string BackupConfigName { get; set; }
        public string SourceDirectoryUNC { get; set; }
        public string TargetDirectoryUNC { get; set; }
        public long FileSize { get; set; }
        public long TimeToCrypt { get; set; }
        public long TransfertTime { get; set; }
    }
}
