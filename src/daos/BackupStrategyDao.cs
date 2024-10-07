using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using prosoft_easysave.src.models;
using System;
using System.IO;
using System.Text;

namespace projet_easy_save_v2.src.daos
{
    class BackupStrategyDao
    {



        // Path to file containing all backup strategies.
        private readonly string backupStrategiesFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
                                                 @"src\resources\config\backup_configs");

        // The key corrsponding to backup strategies in the json file.
        private readonly string backupStrategiesKey = "backupStrategies";

        /// <summary>
        /// Retries the backup strategies as a json array.
        /// </summary>
        /// <returns>The json array retrieved.</returns>
        public JArray GetBackupStrategiesListAsJArray()
        {

            // If the file doesn't exist, create it
            if (!File.Exists(backupStrategiesFilePath))
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append("{\n\t").Append(backupStrategiesKey).Append(": []\n}");
                File.WriteAllText(backupStrategiesFilePath, strBuilder.ToString(), Encoding.UTF8);
            }

            // Returns array of json objects present backupConfigurations array.
            JArray jsonArray = (JArray)GetBackupStrategiesConfigFileAsJsonObject()[backupStrategiesKey];

            if(jsonArray == null)
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append("{\n\t").Append(backupStrategiesKey).Append(": []\n}");
                File.WriteAllText(backupStrategiesFilePath, strBuilder.ToString(), Encoding.UTF8);

                jsonArray = (JArray)GetBackupStrategiesConfigFileAsJsonObject()[backupStrategiesKey];
            }

            return jsonArray;

        }

        /// <summary>
        /// Retrieves the backup strategies list as a json object.
        /// </summary>
        /// <returns>The json object retrieved</returns>
        public JObject GetBackupStrategiesConfigFileAsJsonObject()
        {
            try
            {
                // store all text in json object
                return JObject.Parse(File.ReadAllText(backupStrategiesFilePath));
            } catch
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append("{\n\t").Append("\"").Append(backupStrategiesKey).Append("\": []\n}");
                File.WriteAllText(backupStrategiesFilePath, strBuilder.ToString(), Encoding.UTF8);
                return JObject.Parse(File.ReadAllText(backupStrategiesFilePath));
            }
        }

        /// <summary>
        /// Writes backup strategies in a file as Json Array.
        /// </summary>
        /// <param name="jArray">The json array to write in.</param>
        public void WriteBackupStrategiesFromJArray(JArray jArray)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\n\t").Append(backupStrategiesKey).Append(": [\n");

            foreach(JObject item in jArray)
            {
                strBuilder.Append(item);
                strBuilder.Append(",");
            }
            strBuilder.Append("\n]\n}");


            File.WriteAllText(backupStrategiesFilePath, JsonConvert.SerializeObject(JObject.Parse(strBuilder.ToString()), Formatting.Indented));
        }

        /// <summary>
        /// Removes a backup strategy from config file.
        /// </summary>
        /// <param name="id">The id of the backup strategy.</param>
        public void RemoveBackupStrategy(BackupStrategy backupStrategy)
        {
            JArray jsonArray = GetBackupStrategiesListAsJArray();
            foreach(JObject obj in jsonArray)
            {
                if ((int)obj["Id"] == backupStrategy.Id && obj["CreationDate"].ToString() == backupStrategy.CreationDate)
                {
                    jsonArray.Remove(obj);
                    break;
                }
            }

            WriteBackupStrategiesFromJArray(jsonArray);
        }


        public void UpdateBackupStrategy(BackupStrategy backupStrategyToModify, BackupStrategy newBackupStrategy)
        {
            JArray jsonArray = GetBackupStrategiesListAsJArray();
            foreach (JObject obj in jsonArray)
            {
                if ((int)obj["Id"] == backupStrategyToModify.Id && obj["CreationDate"].ToString() == backupStrategyToModify.CreationDate)
                {
                    obj.Replace(JObject.FromObject(newBackupStrategy));
                    break;
                }
            }
            WriteBackupStrategiesFromJArray(jsonArray);
        }

    }
}
