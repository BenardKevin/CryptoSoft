using Newtonsoft.Json.Linq;
using projet_easy_save_v2.src.daos;
using projet_easy_save_v2.src.models;
using projet_easy_save_v2.src.services;
using prosoft_easysave.src.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace projet_easy_save_v2.src.viewModels
{
    class InitiateBackupViewModel : INotifyPropertyChanged
    {
        private static readonly BackupStrategyDao backupStrategyDao = new BackupStrategyDao();
        private static readonly InitiateBackupService initiateBackupService = new InitiateBackupService();

        private readonly ObservableCollection<BackupStrategy> backupStrategiesList = new ObservableCollection<BackupStrategy>();
        private readonly ObservableCollection<BackupStrategy> initiateBackupQueue = new ObservableCollection<BackupStrategy>();
        private ProgressStatus ps = ProgressStatus.Instance;
        private bool enableButtons = false;
        private string startButton_text = "START";

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        public ObservableCollection<BackupStrategy> BackupStrategiesList
        {
            get
            {
                return this.backupStrategiesList;
            }
        }
        public ObservableCollection<BackupStrategy> InitiateBackupQueue
        {
            get
            {
                return this.initiateBackupQueue;
            }
        }

        public ProgressStatus Ps
        {
            get
            {
                return ps;
            }
        }

        public bool EnableButtons
        {
            get
            {
                return enableButtons;
            }
            set
            {
                enableButtons = value;
                OnPropertyChanged();
            }
        }

        public string StartButton_text
        {
            get
            {
                return startButton_text;
            }
            set
            {
                startButton_text = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public void InitializeViewModel()
        {
            GetBackupStrategiesList();
        }

        public async void StartBackupStrategiesAsync()
        {
            EnableButtons = true;
            StartButton_text = "CANCEL";
            await initiateBackupService.InitiateBackupCall(InitiateBackupQueue.ToList());
            EnableButtons = false;
            StartButton_text = "START";
        }

        public void cancelAllBackups() {
            ProgressStatus.Instance.BackupState = BackupState.CANCELED;
        }

        #region View related
        public void GetBackupStrategiesList()
        {
            backupStrategiesList.Clear();
            try
            {
                // Gets text from file as a JsonObject then retrieves array of backupconfigs
                JArray backupStrategiesJArray = backupStrategyDao.GetBackupStrategiesListAsJArray();
                Collection<BackupStrategy> bckpList = new Collection<BackupStrategy>();

                foreach (JObject item in backupStrategiesJArray)
                {
                    bckpList.Add(item.ToObject<BackupStrategy>());
                }
                foreach (BackupStrategy b in bckpList.OrderByDescending(o => o.CreationDate))
                {
                    backupStrategiesList.Add(b);
                }

            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public void AddBackupToQueue(System.Collections.IList selectedItems)
        {
            List<object> listBackupStrategy = new List<object>();

            for (int i = 0; i < selectedItems.Count; i++)
            {
                listBackupStrategy.Add(selectedItems[i]);
            }

            foreach (BackupStrategy selectedItem in listBackupStrategy)
            {
                initiateBackupQueue.Add(selectedItem);
                backupStrategiesList.Remove(selectedItem);
            }
        }

        public void RemoveBackupToQueue(System.Collections.IList selectedItems)
        {
            List<BackupStrategy> listBackupStrategy = new List<BackupStrategy>();

            listBackupStrategy = backupStrategiesList.ToList<BackupStrategy>();
            backupStrategiesList.Clear();

            for (int i = 0; i < selectedItems.Count; i++)
            {
                listBackupStrategy.Add((BackupStrategy)selectedItems[i]);
            }

            foreach (BackupStrategy selectedItem in listBackupStrategy.OrderByDescending(o => o.CreationDate))
            {
                initiateBackupQueue.Remove(selectedItem);
                backupStrategiesList.Add(selectedItem);
            }


        }

        public void resumeBackup()
        {
            initiateBackupService.resumeBackup();

        }

        public void pauseBackup()
        {
            initiateBackupService.pauseBackup();
        }
        #endregion

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}
