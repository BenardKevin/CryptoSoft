using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace projet_easy_save_v2.src.daos
{
    class ParamsDao
    {
        // Path to file containing all backup strategies.
        private readonly string paramsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
                                                 @"src\resources\config\params");

        public void Write(string language, bool? encryptionIsEnabled, List<string> ExtensionsList)
        {

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\n\t");
            strBuilder.Append($"\"IdCounter\": " + GetIdCounter() + ",\n\t");
            strBuilder.Append($"\"Language\": \"{language}\",\n\t");
            strBuilder.Append($"\"EncryptionIsEnabled\": \"{encryptionIsEnabled}\",\n\t");
            strBuilder.Append($"\"FilesExtensionsToEncrypt\": [");
            for (int i = 0; i < ExtensionsList.Count; i++)
            {
                strBuilder.Append("\"").Append(ExtensionsList[i].Trim()).Append("\",");
            }
            strBuilder.Append("]");
            strBuilder.Append("\n}");
            File.WriteAllText(paramsFilePath, strBuilder.ToString(), UTF8Encoding.UTF8);
        }

        public ObservableCollection<string> GetFileExtensions()
        {
            JObject jObj = JObject.Parse(File.ReadAllText(paramsFilePath));
            string str = jObj["FilesExtensionsToEncrypt"].ToString();

            string[] strTab = str.Replace('[', ' ')
                                 .Replace(']', ' ')
                                 .Replace('\"', ' ')
                                 .Replace(System.Environment.NewLine," ").Split(',');

            ObservableCollection<string> obc = new ObservableCollection<string>();
            foreach (string stri in strTab)
            {
                obc.Add(stri.Trim());
            }

            return obc;
        }

        public int GetIdCounter()
        {
            JObject jObj = JObject.Parse(File.ReadAllText(paramsFilePath));
            int idCounter = 1;

            if (idCounter > 999999)
            {
                idCounter = 1;
            }
            else { 
                JToken jtk = jObj["IdCounter"];

                if (jtk == null)
                {
                    idCounter = 1;
                } else
                {
                    idCounter = (int)jtk;
                }
            }

            return idCounter;
        }

        public bool GetEncryptionIsEnabled()
        {
            JObject jObj = JObject.Parse(File.ReadAllText(paramsFilePath));
            return (bool)jObj["EncryptionIsEnabled"];
        }

        public void IncrementIdCounter()
        {
            JObject jObj = JObject.Parse(File.ReadAllText(paramsFilePath));
            int newId = GetIdCounter() + 1;

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\n\t");
            strBuilder.Append($"\"IdCounter\": " + newId + ",\n\t");
            strBuilder.Append($"\"Language\": \"{jObj["Language"]}\",\n\t");
            strBuilder.Append($"\"EncryptionIsEnabled\": \"{jObj["EncryptionIsEnabled"]}\",\n\t");
            strBuilder.Append($"\"FilesExtensionsToEncrypt\": {jObj["FilesExtensionsToEncrypt"]},\n");
            strBuilder.Append("\n}");
            File.WriteAllText(paramsFilePath, strBuilder.ToString(), UTF8Encoding.UTF8);
        }

        public string GetLanguage()
        {
            JObject jObj = JObject.Parse(File.ReadAllText(paramsFilePath));

            return jObj["Language"].ToString();
        }
    }
}
