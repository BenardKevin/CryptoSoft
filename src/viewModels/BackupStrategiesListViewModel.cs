using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using projet_easy_save_v2.src.daos;
using System.Linq;
using Newtonsoft.Json.Linq;
using prosoft_easysave.src.models;

namespace projet_easy_save_v2.src.viewModels
{
    class BackupStrategiesListViewModel
    {
        private readonly BackupStrategyDao backupStrategyDao = new BackupStrategyDao();
        private readonly ObservableCollection<BackupStrategy> backupStrategiesList = new ObservableCollection<BackupStrategy>();

        public BackupStrategiesListViewModel() {
            GetBackupStrategiesList();
        }

        public ObservableCollection<BackupStrategy> BackupStrategiesList
        {
            get
            {
                return backupStrategiesList;
            }
        }


        /// <summary>
        /// Retrieves the backup strategies list.
        /// </summary>
        /// <returns>The list of backup strategies.</returns>
        public void GetBackupStrategiesList()
        {
            backupStrategiesList.Clear();

            List<BackupStrategy> bckpList = new List<BackupStrategy>();
            try
            {
                // Gets text from file as a JsonObject then retrieves array of backupconfigs
                JArray backupStrategiesJArray = backupStrategyDao.GetBackupStrategiesListAsJArray();
                Collection<BackupStrategy> bckpLists = new Collection<BackupStrategy>();

                foreach (JObject item in backupStrategiesJArray)
                {
                    bckpLists.Add(item.ToObject<BackupStrategy>());
                }
                foreach (BackupStrategy b in bckpLists.OrderByDescending(o => o.CreationDate))
                {
                    backupStrategiesList.Add(b);
                }

            }
            catch
            {
                //TODO ERR
            }
        }

        public void DeleteBackupStrategy(object selectedBackup)
        {
            try
            {
                BackupStrategy backupStrategy = (BackupStrategy)selectedBackup;
                backupStrategyDao.RemoveBackupStrategy(backupStrategy);
                backupStrategiesList.Remove(backupStrategy);
            }
            catch (Exception e)
            {
                //TOD Display error
                Console.WriteLine(e.Message);
            }
        }
    }
}
