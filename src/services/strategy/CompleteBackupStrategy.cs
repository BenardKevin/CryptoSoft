using projet_easy_save_v2.src.daos;
using projet_easy_save_v2.src.models;
using projet_easy_save_v2.src.viewModels;
using prosoft_easysave.src.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace projet_easy_save_v2.src.services.strategy
{
    class CompleteBackupStrategy : IBackupStrategy
    {
        /*DailyLogFileSaveDao dailyLogFileSaveDao = new DailyLogFileSaveDao();
        readonly StatusFileSaveDao statusFileSaveDao = new StatusFileSaveDao();
        readonly ParamsDao paramsDao = new ParamsDao();
        readonly ProcessService processService = new ProcessService();

        StatusFileSaves statusFileSaves;

        readonly Stopwatch stopWatch = new Stopwatch();
        readonly Stopwatch cryptTimer = new Stopwatch();
        private bool firstRun;*/
        // ^^^^ OLD ^^^^ //
        //-----------------------------------------//

        private BackupStrategy strategy;
        private DirectoryInfo sourceDirectory;
        private DirectoryInfo targetDirectory;
        private long filesTotalSize = 0;
        private long currentBytePosition = 0;
        private int fileCounter = 0;
        private int bufferSize = 1024 * 1024;

        // Retrieves extensions list
        private IEnumerable<string> obsCollection;
        private List<string> extensionsStringList;

        // Log file related
        private readonly Stopwatch stopWatch = new Stopwatch();
        private readonly DailyLogFileSaveDao dailyLogFileSaveDao = new DailyLogFileSaveDao();

        // State file related
        private StatusFileSaves statusFileSaves = new StatusFileSaves();
        private readonly StatusFileSaveDao statusFileSaveDao = new StatusFileSaveDao();

        // Cryptosoft related
        private readonly ProcessService processService = new ProcessService();
        private readonly ParamsDao paramsDao = new ParamsDao();
        private readonly Stopwatch cryptTimer = new Stopwatch();


        private static ManualResetEvent _suspendEvent = new ManualResetEvent(true);


        public void startBackup(BackupStrategy strategy, DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {
            this.strategy = strategy;
            ProgressStatus.Instance.BackupName = this.strategy.Name;
            ProgressStatus.Instance.CurrentFile = "";
            this.sourceDirectory = sourceDirectory;
            this.targetDirectory = targetDirectory;
            GetFilesTotalSize();


            // Handles files priority
            obsCollection = paramsDao.GetFileExtensions();
            extensionsStringList = new List<string>(obsCollection);

            ProgressStatus.Instance.BackupState = BackupState.RUNNING;
            copyFiles();
        }

        public void copyFiles()
        {
            FileInfo[] files = sourceDirectory.GetFiles();

            // Creates directory if it doesn't already exist
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            files = getFilesWithPriority(files, extensionsStringList);


            foreach (var file in files)
            {
                if (ProgressStatus.Instance.BackupState == BackupState.CANCELED)
                {
                    break;
                } // FOR CANCELING

                bool bgProcessIsRunning = checkForBackgroundProcess();
                if (bgProcessIsRunning)
                {
                    ProgressStatus.Instance.BackupState = BackupState.PENDING;
                    _suspendEvent.Reset();
                }
                if (bgProcessIsRunning && ProgressStatus.Instance.BackupState == BackupState.PENDING)
                {
                    waitingForResume();
                }

                _suspendEvent.WaitOne(Timeout.Infinite); // FOR PAUSING




                stopWatch.Start(); // TIMER

                if (paramsDao.GetEncryptionIsEnabled())
                {
                        if (extensionsStringList.Contains(file.Extension))
                        {

                        processService.EncryptOrDecrypteFile(file, targetDirectory, cryptTimer);

                        FileStream fsIn = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);

                        ProgressStatus.Instance.CurrentFile += file + "\n";
                        currentBytePosition += fsIn.Length;
                        fsIn.Close();
                        fileCounter += 1;

                    }
                        else
                        {
                            fileByteWriter(file.FullName);
                        }
                }
                else
                {
                    fileByteWriter(file.FullName); // Writer
                }





                #region FILE WRITERS
                // DAILY LOG FILE //

                // Get the elapsed time as a TimeSpan value.
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                stopWatch.Reset();
                TimeSpan timeSpanCryptTimer = cryptTimer.Elapsed;
                cryptTimer.Reset();

                WriteLogs(timeSpanCryptTimer, strategy, file.FullName, ts);

                // STATE FILE
                WriteState(strategy);


                #endregion

            }

            if (ProgressStatus.Instance.BackupState != BackupState.CANCELED)
            {
                foreach (var sourceSubdirectory in sourceDirectory.GetDirectories())
                {
                    var targetSubdirectory = targetDirectory.CreateSubdirectory(sourceSubdirectory.Name);
                    this.sourceDirectory = sourceSubdirectory;
                    this.targetDirectory = targetSubdirectory;
                    copyFiles();
                }

                ProgressStatus.Instance.BackupState = BackupState.COMPLETED;
                WriteState(strategy);

            } else
            {
                ProgressStatus.Instance.CurrentFile = "";
                ProgressStatus.Instance.BackupName = "";
                ProgressStatus.Instance.ProgressPercentage = 0;
            }



        }

        private void GetFilesTotalSize()
        {
            var files = Directory.GetFiles(sourceDirectory.FullName);

            foreach (var file in files)
            {
                FileStream fsIn = new FileStream(file, FileMode.Open, FileAccess.Read);
                filesTotalSize += fsIn.Length;
                fsIn.Close();
            }

            // Gets files in subdirectories aswell
            foreach (var sourceSubdirectory in sourceDirectory.GetDirectories())
            {
                var subFiles = Directory.GetFiles(sourceSubdirectory.FullName);

                foreach (var file in subFiles)
                {
                    FileStream fsIn = new FileStream(file, FileMode.Open, FileAccess.Read);
                    filesTotalSize += fsIn.Length;
                    fsIn.Close();
                }
            }
        }

        private void fileByteWriter(string file)
        {
            ProgressStatus.Instance.CurrentFile += file + "\n";
            FileStream fsIn = new FileStream(file, FileMode.Open, FileAccess.Read);
            FileStream fsOut = new FileStream(Path.Combine(targetDirectory.FullName, Path.GetFileName(file)), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fsOut.SetLength(0);
            byte[] bytes = new byte[bufferSize];
            int readByte;

            while ((readByte = fsIn.Read(bytes, 0, bufferSize)) > 0)
            {
                fsOut.Write(bytes, 0, readByte);
                ProgressStatus.Instance.ProgressPercentage = (int)(((currentBytePosition + fsIn.Position) * 100) / filesTotalSize);
            }

            currentBytePosition += fsIn.Length;

            fsIn.Close();
            fsOut.Close();

            fileCounter += 1;
        }

        public void pauseBackup()
        {
            ProgressStatus.Instance.BackupState = BackupState.PENDING;
            _suspendEvent.Set();
        }

        public void resumeBackup()
        {
            ProgressStatus.Instance.BackupState = BackupState.RUNNING;
            _suspendEvent.Reset();
        }

        private void WriteLogs(TimeSpan timeSpanCryptTimer, BackupStrategy backupStrategy, string filePath, TimeSpan transfertTime)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            DailyLogFileSaves dailyLogFileSaves = new DailyLogFileSaves
            {
                Timestamp = DateTime.Now,
                BackupConfigName = backupStrategy.Name,
                FileSize = fileInfo.Length,
                //TimeToCrypt = (long)timeSpanCryptTimer.TotalMilliseconds,
                TransfertTime = (long)transfertTime.TotalMilliseconds,
                TimeToCrypt = (long)timeSpanCryptTimer.TotalMilliseconds,
                SourceDirectoryUNC = fileInfo.FullName,
                TargetDirectoryUNC = fileInfo.FullName.Replace(sourceDirectory.FullName, targetDirectory.FullName)
            };


            dailyLogFileSaveDao.WriteDailyLogSaves(dailyLogFileSaves);
        }

        private void WriteState(BackupStrategy strategy)
        {
            statusFileSaves.BackupConfigName = strategy.Name;
            statusFileSaves.Timestamp = DateTime.Now;
            statusFileSaves.SourceDirectory = sourceDirectory.FullName;
            statusFileSaves.TargetDirectory = targetDirectory.FullName;
            statusFileSaves.FilesAlreadySaved = fileCounter;
            statusFileSaves.FilesTotalSize = filesTotalSize;
            statusFileSaves.TotalFiles = Directory.GetFiles(sourceDirectory.FullName, "*", SearchOption.AllDirectories).Length;
            statusFileSaves.BackupState = ProgressStatus.Instance.BackupState.ToString();
            statusFileSaves.TotalSizeNotSavedYetFiles = statusFileSaves.FilesTotalSize - currentBytePosition;

            // Write to file
            statusFileSaveDao.WriteBackupStrategies(statusFileSaves);

        }

        private FileInfo[] getFilesWithPriority(FileInfo[] files, List<string> extensionsStringList)
        {
            if (paramsDao.GetEncryptionIsEnabled())
            {
                List<FileInfo> priorityFiles = new List<FileInfo>();
                List<FileInfo> notPriorityFiles = new List<FileInfo>();

                foreach (FileInfo file in files)
                {
                    if (extensionsStringList.Contains(file.Extension))
                    {
                        priorityFiles.Add(file);
                    }
                    else
                    {
                        notPriorityFiles.Add(file);
                    }
                }

                return priorityFiles.Concat(notPriorityFiles).ToArray();
            } else
            {
                return files;
            }
        }

        private bool checkForBackgroundProcess()
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

        }

        private async void waitingForResume()
        {
            await Task.Run(() =>
            {
                while (checkForBackgroundProcess())
                {
                }
                ProgressStatus.Instance.BackupState = BackupState.PENDING;
                _suspendEvent.Set();
            });
        }




        /*
        private void NotifyWorker_ProgressChanged(long value)
        {
            initiateBackupViewModel.NotifyWorker_ProgressChanged(value);
        }


        /// <summary>
        /// <inheritdoc cref="IBackupStrategy.copyFiles(BackupStrategy, DirectoryInfo, DirectoryInfo)"/>
        /// </summary>
        public void CopyFiles(BackupStrategy backupStrategy, DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {
            // Creates directory if it doesn't already exist
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            FileInfo[] filesArray = sourceDirectory.GetFiles();
            if (firstRun)
            {
                statusFileSaves.TotalSizeNotSavedYetFiles = GetDirectorySize(backupStrategy);
                firstRun = false;
            }


            // Copying the files
            foreach (FileInfo file in filesArray)
            {
                IEnumerable<string> obsCollection = paramsDao.GetFileExtensions();
                List<string> extensionsStringList = new List<string>(obsCollection);

                stopWatch.Start();

                if (paramsDao.GetEncryptionIsEnabled())
                {
                    foreach(string extension in extensionsStringList)
                    {
                        if(file.Extension == extension)
                        {
                            processService.EncryptOrDecrypteFile(file, targetDirectory, cryptTimer);
                        }
                        else
                        {
                            file.CopyTo(Path.Combine(targetDirectory.FullName, file.Name), true);
                        }
                    }
                }
                else
                {
                    file.CopyTo(Path.Combine(targetDirectory.FullName, file.Name), true);
                }

                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;
                TimeSpan timeSpanCryptTimer = cryptTimer.Elapsed;


                // --- FILE WRITERS ---
                // DAILY LOG FILE //
                WriteLogs(timeSpanCryptTimer, backupStrategy, file.Length, ts, file.FullName, Path.Combine(targetDirectory.FullName, file.Name));


                // STATE FILE //
                statusFileSaves.BackupConfigName = backupStrategy.Name;
                statusFileSaves.Timestamp = DateTime.Now;
                statusFileSaves.SourceDirectory = sourceDirectory.FullName;
                statusFileSaves.TargetDirectory = targetDirectory.FullName;
                statusFileSaves.FilesAlreadySaved =  Directory.GetFiles(targetDirectory.FullName, "*", SearchOption.AllDirectories).Length;
                // recup directory size
                statusFileSaves.FilesTotalSize = GetDirectorySize(backupStrategy);
                // recup total files size not saved yet
                //Stock total files in directory source 
                statusFileSaves.TotalFiles = filesArray.Length;

                //statusFileSaves.FilesAlreadySaved

                WriteState(file);

                // --- END FILE WRITERS ---
            }

            // Gets the subdirectories
            foreach (var sourceSubdirectory in sourceDirectory.GetDirectories())
            {
                var targetSubdirectory = targetDirectory.CreateSubdirectory(sourceSubdirectory.Name);
                CopyFiles(backupStrategy, sourceSubdirectory, targetSubdirectory);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IBackupStrategy.copyFiles(BackupStrategy, DirectoryInfo, DirectoryInfo)"/>
        /// </summary>
        private void WriteLogs(TimeSpan timeSpanCryptTimer, BackupStrategy backupStrategy, long fileSize, TimeSpan transfertTime, string fileSourceDirectory, string fileTargetDirectory)
        {

            DailyLogFileSaves dailyLogFileSaves = new DailyLogFileSaves
            {
                Timestamp = DateTime.Now,
                BackupConfigName = backupStrategy.Name,
                FileSize = fileSize,
                TimeToCrypt = (long)timeSpanCryptTimer.TotalMilliseconds,
                TransfertTime = (long)transfertTime.TotalMilliseconds,
                SourceDirectoryUNC = fileSourceDirectory,
                TargetDirectoryUNC = fileTargetDirectory
            };


            dailyLogFileSaveDao = new DailyLogFileSaveDao();
            dailyLogFileSaveDao.WriteDailyLogSaves(dailyLogFileSaves);
        }


        private long WriteState(FileInfo file)
        {

            //Baclup state 
            if (statusFileSaves.FilesAlreadySaved != statusFileSaves.TotalFiles)
            {
                statusFileSaves.BackupState = "Actived";
            }
            else
            {
                statusFileSaves.BackupState = "Finished";
            }

            //Recup total files size not saved yet
            statusFileSaves.TotalSizeNotSavedYetFiles -= file.Length;

            // Write to file
            statusFileSaveDao.WriteBackupStrategies(statusFileSaves);

            return statusFileSaves.TotalSizeNotSavedYetFiles;
        }


        private long GetDirectorySize(BackupStrategy backupStrategy)
        {
            // Get array of all file names.
            string[] arrayFilesName = Directory.GetFiles(backupStrategy.SourceDirectory, "*.*");

            // Calculate total bytes of all files in a loop.
            long filesTotalSize = 0;
            foreach (string name in arrayFilesName)
            {
                // Use FileInfo to get length of each file.
                FileInfo info = new FileInfo(name);
                filesTotalSize += info.Length;
            }
            // Return total size
            return filesTotalSize;
        }*/



    }
}
