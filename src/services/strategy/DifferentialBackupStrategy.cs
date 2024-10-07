using projet_easy_save_v2.src.daos;
using projet_easy_save_v2.src.models;
using prosoft_easysave.src.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace projet_easy_save_v2.src.services.strategy
{
    class DifferentialBackupStrategy : IBackupStrategy
    {
        /*DailyLogFileSaveDao dailyLogFileSaveDao = new DailyLogFileSaveDao();
        readonly ParamsDao paramsDao = new ParamsDao();
        readonly StatusFileSaveDao statusFileSaveDao = new StatusFileSaveDao();

        StatusFileSaves statusFileSaves;
        readonly ProcessService processService = new ProcessService();

        readonly Stopwatch stopWatch = new Stopwatch();
        readonly Stopwatch cryptTimer = new Stopwatch();*/

        // ^^^^ OLD ^^^^ //
        // ------------------------------------- //

        private BackupStrategy strategy;
        private DirectoryInfo sourceDirectory;
        private DirectoryInfo targetDirectory;
        private long filesTotalSize = 0;
        private int fileCounter = 0;
        private long currentBytePosition = 0;
        private int bufferSize = 1024 * 1024;

        // Retrieves extensions list
        private IEnumerable<string> obsCollection;
        private List<string> extensionsStringList;

        // Logs related
        private readonly Stopwatch stopWatch = new Stopwatch();
        private readonly DailyLogFileSaveDao dailyLogFileSaveDao = new DailyLogFileSaveDao();

        // State file related
        private int totalFiles = 0;
        private StatusFileSaves statusFileSaves = new StatusFileSaves();
        private readonly StatusFileSaveDao statusFileSaveDao = new StatusFileSaveDao();

        // Cryptosoft related
        private readonly ParamsDao paramsDao = new ParamsDao();

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

            copyFiles();
        }

        public void copyFiles()
        {
            ProgressStatus.Instance.BackupState = BackupState.RUNNING;

            var sourceFiles = Directory.GetFiles(sourceDirectory.FullName);
            var targetFiles = Directory.GetFiles(targetDirectory.FullName);



            // Creates directory if it doesn't already exist
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            sourceFiles = getFilesWithPriority(sourceFiles, extensionsStringList).Select(f => f.FullName).ToArray();


            foreach (var sourceFile in sourceFiles)
            {
                bool shouldBeSaved = true;
                var filename = sourceFile.Replace(sourceDirectory.FullName, targetDirectory.FullName);

                stopWatch.Start();

                foreach (var targetFile in targetFiles)
                {

                    if (shouldBeSaved == false)
                    {
                        break;
                    }

                    if (ProgressStatus.Instance.BackupState == BackupState.CANCELED)
                    {
                        break;
                    }

                    _suspendEvent.WaitOne(Timeout.Infinite);

                    if (filename == targetFile)
                    {
                        // Don't save target file if already exists.
                        shouldBeSaved = false;
                        // Save target file if modified.
                        if (File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(targetFile))
                        {
                            fileByteWriter(sourceFile);
                        }
                    }
                }
                // even if there are no files at root
                if (shouldBeSaved)
                {
                    fileByteWriter(sourceFile);
                }

                #region FILE WRITERS
                // DAILY LOG FILE //

                // Get the elapsed time as a TimeSpan value.
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                stopWatch.Reset();

                WriteLogs(strategy, sourceFile, ts);

                // STATE FILE

                WriteState(strategy);



                #endregion-

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
            }
            else
            {
                ProgressStatus.Instance.CurrentFile = "";
                ProgressStatus.Instance.BackupName = "";
                ProgressStatus.Instance.ProgressPercentage = 0;
            }
        }



        private void GetFilesTotalSize()
        {
            var sourceFiles = Directory.GetFiles(sourceDirectory.FullName);
            var targetFiles = Directory.GetFiles(targetDirectory.FullName);

            foreach (var sourceFile in sourceFiles)
            {
                bool shouldBeSaved = true;
                var filename = sourceFile.Replace(sourceDirectory.FullName, targetDirectory.FullName);

                foreach (var targetFile in targetFiles)
                {
                    if(shouldBeSaved == false)
                    {
                        break;
                    }

                    if (filename == targetFile)
                    {
                        // Don't save target file if already exists.
                        shouldBeSaved = false;
                        if (File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(targetFile))
                        {
                            addFilesSizeToTotal(sourceFile);
                        }
                    }
                }
                // even if there are no files at root
                if (shouldBeSaved)
                {
                    addFilesSizeToTotal(sourceFile);
                }

            }

            // Gets files in subdirectories aswell
            foreach (var sourceSubdirectory in sourceDirectory.GetDirectories())
            {
                DirectoryInfo subTargetPath = new DirectoryInfo(Path.Combine(targetDirectory.FullName, sourceSubdirectory.Name));

                if (!subTargetPath.Exists)
                {
                    Directory.CreateDirectory(subTargetPath.FullName);
                }

                var subSourceFiles = sourceSubdirectory.GetFiles();
                var subTargetFiles = subTargetPath.GetFiles();

                foreach (var sourceFile in subSourceFiles)
                {
                    bool shouldBeSaved = true;
                    var filename = sourceFile.FullName.Replace(sourceDirectory.FullName, targetDirectory.FullName);

                    foreach (var targetFile in subTargetFiles)
                    {
                        if (shouldBeSaved == false)
                        {
                            break;
                        }


                        if (filename == targetFile.FullName)
                        {
                            // Don't save target file if already exists.
                            shouldBeSaved = false;
                            if (sourceFile.LastWriteTime > targetFile.LastWriteTime)
                            {
                                addFilesSizeToTotal(sourceFile.FullName);
                            }
                        }

                    }
                    // even if there are no files at root
                    if (shouldBeSaved)
                    {
                        addFilesSizeToTotal(sourceFile.FullName);
                    }
                }
            }
        }

        public void pauseBackup()
        {
            _suspendEvent.Set();
        }

        public void resumeBackup()
        {
            _suspendEvent.Reset();
        }

        private void addFilesSizeToTotal(string file)
        {
            FileStream fsIn = new FileStream(file, FileMode.Open, FileAccess.Read);
            filesTotalSize += fsIn.Length;
            fsIn.Close();
            totalFiles += 1;
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

        private void WriteLogs(BackupStrategy backupStrategy, string filePath, TimeSpan transfertTime)
        {

            FileInfo fileInfo = new FileInfo(filePath);

            DailyLogFileSaves dailyLogFileSaves = new DailyLogFileSaves
            {
                Timestamp = DateTime.Now,
                BackupConfigName = backupStrategy.Name,
                FileSize = fileInfo.Length,
                //TimeToCrypt = (long)timeSpanCryptTimer.TotalMilliseconds,
                TransfertTime = (long)transfertTime.TotalMilliseconds,
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
            statusFileSaves.TotalFiles = totalFiles;
            statusFileSaves.BackupState = ProgressStatus.Instance.BackupState.ToString();
            statusFileSaves.TotalSizeNotSavedYetFiles = statusFileSaves.FilesTotalSize - currentBytePosition;

            // Write to file
            statusFileSaveDao.WriteBackupStrategies(statusFileSaves);

        }

        private FileInfo[] getFilesWithPriority(string[] files, List<string> extensionsStringList)
        {
            List<FileInfo> filesList = new List<FileInfo>();

            foreach (var file in files)
            {
                filesList.Add(new FileInfo(file));
            }

            if (paramsDao.GetEncryptionIsEnabled())
            {

                List<FileInfo> priorityFiles = new List<FileInfo>();
                List<FileInfo> notPriorityFiles = new List<FileInfo>();

                foreach (FileInfo file in filesList)
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
            }
            else
            {
                return filesList.ToArray();
            }
        }

        /*

        public void CopyFiles(BackupStrategy backupStrategy, DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {

            FileInfo[] filesArray = sourceDirectory.GetFiles("*", SearchOption.AllDirectories);

            statusFileSaves.TotalSizeNotSavedYetFiles = statusFileSaves.FilesTotalSize;

            // Copying the files
            foreach (FileInfo sourceFile in filesArray)
            {
                bool shouldBeSaved = true;
                var filename = sourceFile.FullName.Replace(sourceDirectory.FullName, targetDirectory.FullName);

                foreach (FileInfo targetFile in targetDirectory.GetFiles("*", SearchOption.AllDirectories))
                {
                    IEnumerable<string> obsCollection = paramsDao.GetFileExtensions();
                    List<string> extensionsStringList = new List<string>(obsCollection);

                    stopWatch.Start();

                    if (filename == targetFile.FullName)
                    {
                        // Don't save target file if already exists.
                        shouldBeSaved = false;
                        // Save target file if modified.
                        if(sourceFile.LastWriteTime > targetFile.LastWriteTime)
                        {
                            if (paramsDao.GetEncryptionIsEnabled())
                            {
                                foreach (string extension in extensionsStringList)
                                {
                                    if (sourceFile.Extension == extension)
                                    {
                                        processService.EncryptOrDecrypteFile(sourceFile, targetDirectory, cryptTimer);
                                    }
                                    else
                                    {
                                        sourceFile.CopyTo(filename, true);
                                    }
                                }
                            } else
                            {
                                sourceFile.CopyTo(filename, true);
                            }

                        }
                    }

                    // Copy if target file doesn't exist.
                    if (shouldBeSaved)
                    {
                        if (paramsDao.GetEncryptionIsEnabled())
                        {
                        }
                        else
                        {
                            sourceFile.CopyTo(filename, true);
                        }
                    }

                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;
                    TimeSpan timeSpanCryptTimer = cryptTimer.Elapsed;

                    // --- FILE WRITERS ---

                    // DAILY LOG FILE
                    WriteLogs(timeSpanCryptTimer, backupStrategy, sourceFile.Length, ts, sourceFile.FullName, Path.Combine(targetDirectory.FullName, sourceFile.Name));

                    // STATE FILE //
                    statusFileSaves.BackupConfigName = backupStrategy.Name;
                    statusFileSaves.Timestamp = DateTime.Now;
                    statusFileSaves.SourceDirectory = sourceDirectory.FullName;
                    statusFileSaves.TargetDirectory = targetDirectory.FullName;
                    statusFileSaves.FilesAlreadySaved = Directory.GetFiles(targetDirectory.FullName, "*", SearchOption.AllDirectories).Length;
                    // recup directory size
                    statusFileSaves.FilesTotalSize = GetDirectorySize(backupStrategy);
                    // recup total files size not saved yet
                    //Stock total files in directory source 
                    statusFileSaves.TotalFiles = filesArray.Length;

                    //statusFileSaves.FilesAlreadySaved

                    statusFileSaves.TotalSizeNotSavedYetFiles = WriteState(sourceFile);

                    // --- END FILE WRITERS ---

                }
            }

            // Gets the subdirectories
            foreach (DirectoryInfo sourceSubdirectory in sourceDirectory.GetDirectories())
            {
                DirectoryInfo targetSubdirectory = targetDirectory.CreateSubdirectory(sourceSubdirectory.Name);
                CopyFiles(backupStrategy, sourceSubdirectory, targetSubdirectory);
            }
        }

        private void WriteLogs(TimeSpan timeToCrypt, BackupStrategy backupStrategy, long fileSize, TimeSpan transfertTime, string fileSourceDirectory, string fileTargetDirectory)
        {
            DailyLogFileSaves dailyLogFileSaves = new DailyLogFileSaves
            {
                Timestamp = DateTime.Now,
                BackupConfigName = backupStrategy.Name,
                FileSize = fileSize,
                TransfertTime = (long)transfertTime.TotalMilliseconds,
                TimeToCrypt = (long)timeToCrypt.TotalMilliseconds,
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
            statusFileSaves.TotalSizeNotSavedYetFiles -= file.Length; ;

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
        }

        public void startBackup(BackupStrategy strategy, DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {
            throw new NotImplementedException();
        }

        public void pauseBackup()
        {
            throw new NotImplementedException();
        }

        public void resumeBackup()
        {
            throw new NotImplementedException();
        }*/
    }
}
