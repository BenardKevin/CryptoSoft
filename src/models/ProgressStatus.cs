using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace projet_easy_save_v2.src.models
{
    public enum BackupState
    {
        NONE,
        RUNNING,
        PENDING,
        CANCELED,
        FAILED,
        COMPLETED
    }

    sealed class ProgressStatus: INotifyPropertyChanged
    {
        private static readonly object padlock = new object();
        private static ProgressStatus instance = null;
        public static ProgressStatus Instance
        {
            get
            {
                lock (padlock)
                {
                    if(instance == null)
                    {
                        instance = new ProgressStatus();
                    }
                    return instance;
                }
            }
        }

        private int progressPercentage;
        private BackupState backupState;
        private string backupName;
        private string currentFile;

        public event PropertyChangedEventHandler PropertyChanged;


        public int ProgressPercentage
        {
            get
            {
                return progressPercentage;
            }
            set
            {
                progressPercentage = value;
                OnPropertyChanged();
            }
        }
        public BackupState BackupState
        {
            get
            {
                return backupState;
            }
            set
            {
                backupState = value;
                OnPropertyChanged();
            }
        }
        public string BackupName
        {
            get
            {
                return backupName;
            }
            set
            {
                backupName = value;
                OnPropertyChanged();
            }
        }
        public string CurrentFile
        {
            get
            {
                return currentFile;
            }
            set
            {
                currentFile = value;
                OnPropertyChanged();
            }
        }


        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}
