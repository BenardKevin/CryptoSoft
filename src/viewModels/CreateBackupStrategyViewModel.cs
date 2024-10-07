using Newtonsoft.Json.Linq;
using projet_easy_save_v2.src.daos;
using prosoft_easysave.src.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace projet_easy_save_v2.src.viewModels
{
    class CreateBackupStrategyViewModel : INotifyPropertyChanged
    {
        private readonly BackupStrategyDao backupStrategyDao = new BackupStrategyDao();
        private readonly ParamsDao paramsDao = new ParamsDao();
        private string sourceDirectory;
        private string targetDirectory;
        private readonly string[] enumNames;


        public event PropertyChangedEventHandler PropertyChanged;

        public CreateBackupStrategyViewModel()
        {
            enumNames = Enum.GetNames(typeof(TypeOfSave));
        }

        public string[] EnumNames
        {
            get
            {
                return enumNames;
            }
        }
        public string SourceDirectory
        {
            get
            {
                return sourceDirectory;
            }
            set
            {
                sourceDirectory = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public string TargetDirectory
        {
            get
            {
                return targetDirectory;
            }
            set
            {
                targetDirectory = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void CreateBackupStrategy(string name, string type, string sourceDirectory, string targetDirectory)
        {
            try
            {
                DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);
                DirectoryInfo targetDirectoryInfo = new DirectoryInfo(targetDirectory);

                if (!sourceDirectoryInfo.Exists)
                {
                    throw new DirectoryNotFoundException();
                }

                JArray backupStrategiesJArray = backupStrategyDao.GetBackupStrategiesListAsJArray();

                JObject backupStrategy = new JObject(new JProperty("Id", paramsDao.GetIdCounter()),
                                                         new JProperty("CreationDate", DateTime.Now.ToString("G")),
                                                         new JProperty("Name", name),
                                                         new JProperty("TypeOfSave", (TypeOfSave)Enum.Parse(typeof(TypeOfSave), type.ToString())),
                                                         new JProperty("SourceDirectory", sourceDirectory),
                                                         new JProperty("TargetDirectory", targetDirectory));

                        backupStrategiesJArray.Add(backupStrategy);
                        backupStrategyDao.WriteBackupStrategiesFromJArray(backupStrategiesJArray);
                paramsDao.IncrementIdCounter();
                }
            catch
            {
                //TODO throw err
            }
        }

    }
}
