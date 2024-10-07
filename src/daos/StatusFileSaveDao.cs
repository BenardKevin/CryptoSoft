using Newtonsoft.Json;
using prosoft_easysave.src.models;
using System;
using System.IO;

namespace projet_easy_save_v2.src.daos
{
    class StatusFileSaveDao
    {
        // Path to file containing all backup strategies.
        private readonly string stateFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"src\resources\");

        public void WriteBackupStrategies(StatusFileSaves statusFileSaves)
        {
            if (!(Directory.Exists(stateFilePath)))
            {
                Directory.CreateDirectory(stateFilePath);
            }
            string stateFileContent = string.Concat(JsonConvert.SerializeObject(statusFileSaves, Formatting.Indented));
            
            File.WriteAllText(Path.Combine(stateFilePath, "state.json"), stateFileContent);
        }
    }
}
