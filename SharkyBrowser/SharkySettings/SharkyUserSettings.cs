
using SharkyBrowser.SharkyWeb;
using System;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace SharkyBrowser.SharkySettings
{
    public class SharkyUserSettings
    {
        public static readonly string SettingsFileLocation = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\Sharky\\usersettings.xml");
        private static SharkyUserSettings CurrentInstance;
        public static SharkyUserSettings Instance
        {
            get 
            {
                if(CurrentInstance == null)
                {
                    if (File.Exists(SettingsFileLocation))
                    {
                        StreamReader settingsFile = new(SettingsFileLocation);
                        XmlSerializer xmlSerializer = new(typeof(SharkyUserSettings));
                        CurrentInstance = (SharkyUserSettings)xmlSerializer.Deserialize(settingsFile);
                        settingsFile.Close();
                    } else
                    {
                        CurrentInstance = new();
                    }
                }

                return CurrentInstance;
            }
        }

        public string Homepage = "https://start.lesmajesticiels.org";
        public string SearchUrl = SharkyWebSearchEngine.DefaultEngines[0].UrlPattern;
        public string DownloadLocation = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "\\Downloads");
        public bool MultipleTabWarningOnClose = true;

        public string GlobalCookiePolicy = "Accept";

        /// <summary>
        /// Envoyer les entêtes anti tracking (Do Not Track)
        /// </summary>
        public bool SendDNTHeaders = true;

        /// <summary>
        /// Bloquer les requêtes vers des sites connus pour le tracking
        /// </summary>
        public bool BlockTrackingRequests = false;

        /// <summary>
        /// Bloquer la navigation vers les sites pour adultes
        /// </summary>
        public bool BlockAdultWebsites = false;

        /// <summary>
        /// Bloquer les publicités
        /// </summary>
        public bool BlockAdvertisements = false;

        /// <summary>
        /// Bloquer le contenu parasite
        /// </summary>
        public bool BlockAnnoyances = false;

        /// <summary>
        /// Bloquer les sites malveillants
        /// </summary>
        public bool BlockBadware = true;

        public string AcceptLanguage = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;

        public void WriteToFile()
        {
            StreamWriter settingsFile = new(SettingsFileLocation);
            XmlSerializer xmlSerializer = new(typeof(SharkyUserSettings));
            xmlSerializer.Serialize(settingsFile, this);
            settingsFile.Close();
        }
    }
}
