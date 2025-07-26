using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Web.Http;

namespace SharkyBrowser.SharkyFilter
{
    /// <summary>
    /// Ce filtre de contenu est basé sur les noms de domaines.
    /// Une liste noire de domaines est utilisée pour filtrer les contenus indésirables.
    /// </summary>
    internal class SharkyDomainFilter
    {
        /// <summary>
        /// Chemin local de la liste noire de domaines.
        /// </summary>
        protected string BlacklistLocalPath;

        /// <summary>
        /// Chemin distant de la liste noire de domaines.
        /// </summary>
        protected string BlacklistRemotePath;

        /// <summary>
        /// Liste noire de domaines.
        /// </summary>
        protected List<String> Blacklist;

        public SharkyDomainFilter(string blacklistLocalPath, string blacklistRemotePath)
        {
            BlacklistLocalPath = blacklistLocalPath;
            BlacklistRemotePath = blacklistRemotePath;
            Blacklist = [];

            // Initialiser la liste noire depuis le chemin local
            if (!string.IsNullOrEmpty(blacklistLocalPath) && File.Exists(blacklistLocalPath))
            {
                try
                {
                    string domains = File.ReadAllText(blacklistLocalPath);
                    Blacklist = [.. domains.Split(",")];
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la lecture de la liste noire locale : {ex.Message}");
                }
            }

            UpdateList();
        }

        /// <summary>
        /// Mettre à jour la liste depuis le chemin distant
        /// </summary>
        protected async void UpdateList()
        {
            HttpClient client = new();
            try
            {
                HttpResponseMessage msg = await client.GetAsync(new Uri(BlacklistRemotePath));
                string domains = await msg.Content.ReadAsStringAsync();

                Blacklist = [.. domains.Split(",")];

                // Enregistrer la liste noire dans le chemin local
                if (!string.IsNullOrEmpty(BlacklistLocalPath))
                {
                    File.WriteAllText(BlacklistLocalPath, string.Join(",", Blacklist));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Teste si un domaine se trouve dans la liste noire.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool TestDomain(string domain)
        {
            if (Blacklist == null || Blacklist.Count == 0)
            {
                UpdateList();
                return false;
            }

            // Vérifier si le domaine est dans la liste noire
            return Blacklist.Any(d => domain.Contains(d, StringComparison.OrdinalIgnoreCase));
        }
    }
}
