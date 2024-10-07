using projet_easy_save_v2.src.daos;
using projet_easy_save_v2.src.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace projet_easy_save_v2.src.viewModels
{
    class ParamsViewModel
    {
        private readonly ParamsDao paramsDao = new ParamsDao();
        private readonly ObservableCollection<string> fileExtensionsCollection = new ObservableCollection<string>();
        private bool encryptionIsEnabled;
        private string currentLanguage;
        private string oldLanguage;
        private readonly string[] enumLanguages;

        public ParamsViewModel()
        {
            enumLanguages = Enum.GetNames(typeof(Languages));
            currentLanguage = paramsDao.GetLanguage();
            oldLanguage = currentLanguage;
            fileExtensionsCollection = paramsDao.GetFileExtensions();
            encryptionIsEnabled = paramsDao.GetEncryptionIsEnabled();
        }

        public string CurrentLanguage
        {
            get
            {
                return currentLanguage;
            }
            set
            {
                currentLanguage = value;
            }
        }

        public string[] EnumLanguages
        {
            get
            {
                return enumLanguages;
            }
        }

        public bool EncryptionIsEnabled
        {
            get
            {
                return encryptionIsEnabled;
            }
            set
            {
                encryptionIsEnabled = value;
            }
        }

        public ObservableCollection<string> FileExtensionsCollection
        {
            get
            {
                return fileExtensionsCollection;
            }
        }

        public bool SaveParams(string language, bool? EncryptionIsEnabled, ItemCollection filesExtensionsToEncrypt)
        {
            List<string> selectedFields = new List<string>();

            foreach (object a in filesExtensionsToEncrypt)
            {
                selectedFields.Add(a.ToString());
            }

            paramsDao.Write(language, EncryptionIsEnabled, selectedFields);

            if (language != oldLanguage)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public void AddFileExtension(string text)
        {
            if (text != null)
            {
                this.fileExtensionsCollection.Add(text);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public void RemoveFileExtension(object selectedItem)
        {
            fileExtensionsCollection.Remove(selectedItem.ToString());
        }
    }
}
