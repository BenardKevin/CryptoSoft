using prosoft_easysave.src.models;
using System;
using System.ComponentModel;
using System.IO;

namespace projet_easy_save_v2.src.services.strategy
{
    class BackupStrategyManager
    {
        private IBackupStrategy _backupStrategy;

        /// <summary>
        /// Sets the strategy corresponding to the type of save.
        /// </summary>
        /// <param name="backupStrategy">
        /// <inheritdoc cref="IBackupStrategy"/>
        /// </param>
        public void SetStrategy(IBackupStrategy backupStrategy)
        {
            _backupStrategy = backupStrategy;
        }

        /// <summary>
        /// Calls copyFiles corresponding to the corresponding strategy.
        /// </summary>
        /// <param name="backupStrategy">The backup strategy.</param>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="targetDirectory">The target Directory</param>
        public void startBackup(BackupStrategy strategy, DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {
            _backupStrategy.startBackup(strategy, sourceDirectory, targetDirectory);
        }

        public void pauseBackup()
        {
            _backupStrategy.pauseBackup();
        }

        public void resumeBackup()
        {
            _backupStrategy.resumeBackup();
        }

        public void copyFiles()
        {
            _backupStrategy.copyFiles();
        }

    }

}
