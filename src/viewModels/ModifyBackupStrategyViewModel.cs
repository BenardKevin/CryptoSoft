using System;
using projet_easy_save_v2.src.daos;
using System.ComponentModel;
using prosoft_easysave.src.models;

namespace projet_easy_save_v2.src.viewModels
{
    class ModifyBackupStrategyViewModel
    {
        private readonly BackupStrategyDao backupStrategyDao = new BackupStrategyDao();
        private readonly BackupStrategy backupStrategyToModify = new BackupStrategy();
        private readonly string[] enumNames;

        public event PropertyChangedEventHandler PropertyChanged;

        public ModifyBackupStrategyViewModel(object selectedBackup) {
            backupStrategyToModify = (BackupStrategy)selectedBackup;

            enumNames = Enum.GetNames(typeof(TypeOfSave));
        }

        public BackupStrategy BackupStrategyToModify
        {
            get {
                return backupStrategyToModify;
            }
        }

        public string[] EnumNames
        {
            get
            {
                return enumNames;
            }
        }


        public void UpdateBackupStrategy(string newName, string newTypeOfSave, string newSource, string newTTarget)
        {
            try
            {
                BackupStrategy newBackupStrategy = new BackupStrategy
                {
                    Id = backupStrategyToModify.Id,
                    CreationDate = DateTime.Now.ToString("G"),
                    Name = newName,
                    TypeOfSave = (TypeOfSave)Enum.Parse(typeof(TypeOfSave), (newTypeOfSave.ToString())),
                    SourceDirectory = newSource,
                    TargetDirectory = newTTarget
                };

                backupStrategyDao.UpdateBackupStrategy(backupStrategyToModify, newBackupStrategy);

            } catch
            {
                //TODO Throw Exception;
            }
        }
    }
}
