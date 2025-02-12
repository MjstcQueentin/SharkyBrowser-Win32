using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SharkyBrowser.SharkyWeb
{
    /// <summary>
    /// Describes a search engine
    /// </summary>
    public class SharkyWebSearchEngine
    {
        public static readonly string SearchEnginesFileLocation = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\Sharky\\searchengines.xml");

        /// <summary>
        /// Name of the search engine
        /// </summary>
        public string Name;

        /// <summary>
        /// URL that points to the search engine results page.
        /// %s is where the search query will be placed, after being URL-encoded.
        /// </summary>
        public string UrlPattern;

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Default search engines
        /// </summary>
        public static List<SharkyWebSearchEngine> DefaultEngines
        {
            get
            {
                List<SharkyWebSearchEngine> list = new()
                {
                    new SharkyWebSearchEngine
                    {
                        Name = "Qwant",
                        UrlPattern = "https://www.qwant.com/?q=%s"
                    },
                    new SharkyWebSearchEngine
                    {
                        Name = "Google",
                        UrlPattern = "https://www.google.com/search?q=%s"
                    }
                };

                return list;
            }
        }

        public static List<SharkyWebSearchEngine> UserSavedEngines
        {
            get
            {
                List<SharkyWebSearchEngine> list = DefaultEngines;

                if (File.Exists(SearchEnginesFileLocation))
                {
                    StreamReader searchEnginesFile = new(SearchEnginesFileLocation);
                    XmlSerializer xmlSerializer = new(typeof(List<SharkyWebSearchEngine>));
                    list = (List<SharkyWebSearchEngine>)xmlSerializer.Deserialize(searchEnginesFile);
                    searchEnginesFile.Close();
                }

                return list;
            }
        }

        private static void WriteToFile(List<SharkyWebSearchEngine> list)
        {
            StreamWriter searchEnginesFile = new(SearchEnginesFileLocation);
            XmlSerializer xmlSerializer = new(typeof(List<SharkyWebSearchEngine>));
            xmlSerializer.Serialize(searchEnginesFile, list);
            searchEnginesFile.Close();
        }

        public static List<SharkyWebSearchEngine> AddEngineToList(string name, string urlPattern)
        {
            List<SharkyWebSearchEngine> list = UserSavedEngines;
            list.Add(new SharkyWebSearchEngine
            {
                Name = name,
                UrlPattern = urlPattern
            });

            WriteToFile(list);

            return list;
        }

        public static List<SharkyWebSearchEngine> RemoveEngineFromList(int index)
        {
            List<SharkyWebSearchEngine> list = UserSavedEngines;
            list.RemoveAt(index);

            WriteToFile(list);

            return list;
        }
    }
}
