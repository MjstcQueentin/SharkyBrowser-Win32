using SharkyBrowser.SharkyFilter;
using SharkyBrowser.SharkySettings;
using System;
using System.Collections.Generic;

namespace SharkyBrowser.SharkyWeb
{
    /// <summary>
    /// Permet le filtrage de contenus dans Sharky (fonctions anti-publicitaires, anti-badware, anti-ennuis, contrôle parental)
    /// </summary>
    internal class SharkyWebFilter
    {
        private static List<SharkyDomainFilter> DomainFilters;

        public static void Initialize()
        {
            DomainFilters = [];

            if(SharkyUserSettings.Instance.BlockAdultWebsites)
            {
                DomainFilters.Add(new("", "http://sharky.lesmajesticiels.org/lists/domains/adult.txt"));
            }

            if(SharkyUserSettings.Instance.BlockAdvertisements)
            {
                DomainFilters.Add(new("", "http://sharky.lesmajesticiels.org/lists/domains/advertisement.txt"));
            }

            if(SharkyUserSettings.Instance.BlockAnnoyances)
            {
                DomainFilters.Add(new("", "http://sharky.lesmajesticiels.org/lists/domains/annoyance.txt"));
            }
        }

        /// <summary>
        /// Teste si une URI devrait être bloquée ou non.
        /// </summary>
        /// <param name="uri">URI a tester</param>
        /// <returns>TRUE si l'URI matche avec au moins un filtre, FALSE sinon</returns>
        public static bool TestUri(Uri uri)
        {
            // Tester d'abord le domaine
            foreach (var item in DomainFilters)
            {
                if (item.TestDomain(uri.Host)) return true;
            }

            // Aucun filtre n'a matché
            return false;
        }
    }
}
