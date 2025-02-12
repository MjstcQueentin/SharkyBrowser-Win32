using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharkyBrowser.SharkyWeb
{
    /// <summary>
    /// Permet le filtrage de contenus dans Sharky (fonctions anti-publicitaires, anti-badware, anti-ennuis, contrôle parental)
    /// </summary>
    internal class SharkyWebFilter
    {
        private static List<string> AdultBlacklist;
        private static List<string> BadwareBlacklist;
        private static List<string> AdvertisementBlacklist;
        private static List<string> AnnoyanceBlacklist;

        /// <summary>
        /// Updates lists for use by content filters
        /// </summary>
        public static async void UpdateLists()
        {
            HttpClient client = new();
            try
            {
                HttpResponseMessage adultMsg = await client.GetAsync("http://api-sharky.lesmajesticiels.localhost/lists/adult.txt");
                string adultDomains = await adultMsg.Content.ReadAsStringAsync();

                AdultBlacklist = adultDomains.Split(",").ToList();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public static bool CheckList(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return false;

            if (AdultBlacklist.Exists(domain => domain.Contains(query)))
            {
                return true;
            }

            return false;
        }
    }
}
