using prosoft_easysave.src.models;
using System.ComponentModel;
using System.IO;

namespace projet_easy_save_v2.src.services.strategy
{
    /// <summary>
    /// Interface reference for Strategy design pattern.
    /// </summary>
    public interface IBackupStrategy
    {
        void startBackup(BackupStrategy strategy, DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory);

        void pauseBackup();
        void resumeBackup();
        void copyFiles();
    }
}
