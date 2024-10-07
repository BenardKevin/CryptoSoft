using projet_easy_save_v2.src.models;
using projet_easy_save_v2.src.services.strategy;
using projet_easy_save_v2.src.viewModels;
using prosoft_easysave.src.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace projet_easy_save_v2.src.services
{
    class InitiateBackupService
    {
        private readonly BackupStrategyManager backupStrategyManager = new BackupStrategyManager();


        public void StartBackupStrategy(BackupStrategy backupStrategy)
        {
            DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(backupStrategy.SourceDirectory);
            DirectoryInfo targetDirectoryInfo = new DirectoryInfo(backupStrategy.TargetDirectory);

            /*
            if (GetProcessRunning())
            {
                throw new TaskCanceledException();
            }*/

            if (backupStrategy.TypeOfSave == TypeOfSave.COMPLETE)
            {
                backupStrategyManager.SetStrategy(new CompleteBackupStrategy());
            }
            else if (backupStrategy.TypeOfSave == TypeOfSave.DIFFERENTIAL)
            {
                if (!targetDirectoryInfo.Exists)
                {
                    targetDirectoryInfo.Create();
                }
                if (targetDirectoryInfo.GetFiles("*", SearchOption.AllDirectories).Length == 0)
                {
                    backupStrategyManager.SetStrategy(new CompleteBackupStrategy());
                }
                else
                {
                    backupStrategyManager.SetStrategy(new DifferentialBackupStrategy());
                }

            }

            backupStrategyManager.startBackup(backupStrategy, sourceDirectoryInfo, targetDirectoryInfo);

        }

        public void pauseBackup()
        {
            ProgressStatus.Instance.BackupState = BackupState.PENDING;

            backupStrategyManager.resumeBackup();
        }

        public void resumeBackup()
        {
            ProgressStatus.Instance.BackupState = BackupState.RUNNING;

            backupStrategyManager.pauseBackup();
        }

        public async Task<bool> InitiateBackupCall(List<BackupStrategy> backupStrategies)
        {
            await Task.Run(() =>
            {
                if (backupStrategies.Count == 1)
                {
                    // Start a single backup strategy
                    StartBackupStrategy(backupStrategies[0]);

                }

                else if (backupStrategies.Count > 1)
                {
                    foreach (BackupStrategy bs in backupStrategies)
                    {
                        do
                        {

                            if(ProgressStatus.Instance.BackupState != BackupState.RUNNING)
                            {
                                StartBackupStrategy(bs);
                            }

                        } while (ProgressStatus.Instance.BackupState == BackupState.RUNNING);

                        if(ProgressStatus.Instance.BackupState == BackupState.CANCELED)
                        {
                            break;
                        }
                    }

                }
                else
                {
                    // Throw error if queue is empty
                    throw new ArgumentNullException();
                }
            });

            return true;
        }

        /*
        private bool GetProcessRunning()
        {
            Process[] pname = Process.GetProcesses();

            Process[] calculators = Process.GetProcessesByName("calculator");

            if (pname.Length > 0 && calculators.Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }*/
    }
}
