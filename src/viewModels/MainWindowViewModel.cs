using projet_easy_save_v2.src.daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projet_easy_save_v2.src.viewModels
{
    class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            ParamsDao paramsDao = new ParamsDao();
            string currentLanguage = paramsDao.GetLanguage();
            string currentLanguageToCulture = currentLanguage.Substring(currentLanguage.Length - 2, 2).ToLower();
            if(currentLanguageToCulture == "fr")
            {
                // Sets current language to french
                System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo("fr");
            } else if (currentLanguageToCulture == "en")
            {
                // Sets current language to english
                System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo("en");
            }
        }
    }
}
