using Newtonsoft.Json;
using prosoft_easysave.src.models;
using System;
using System.IO;

namespace projet_easy_save_v2.src.daos
{
    class DailyLogFileSaveDao
    {
        // Path to file containing all backup strategies.
        private readonly string backupStrategiesFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                 $@"EasySave\logs\");

        /// <summary>
        /// Writes save in the daily log file.
        /// </summary>
        /// <param name="dailyLogFileSaves">The object to save to file.</param>
        public void WriteDailyLogSaves(DailyLogFileSaves dailyLogFileSaves)
        {
            if (!Directory.Exists(backupStrategiesFilePath))
            {
                Directory.CreateDirectory(backupStrategiesFilePath);
            }

            File.AppendAllText(Path.Combine(backupStrategiesFilePath, $"backup_log[{ DateTime.Now:yyyy-MM-dd}].log"), string.Concat(JsonConvert.SerializeObject(dailyLogFileSaves, Formatting.Indented), ","));
        }

    }
}
