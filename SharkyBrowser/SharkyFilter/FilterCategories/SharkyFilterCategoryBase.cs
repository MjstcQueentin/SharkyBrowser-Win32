using SharkyBrowser.SharkyFilter.FilterTypes;
using System;
using System.Collections.Generic;

namespace SharkyBrowser.SharkyFilter.FilterCategories
{
    /// <summary>
    /// Describes a base class for Sharky filters.
    /// </summary>
    internal class SharkyFilterCategoryBase
    {
        public string Name = "Untitled Filter Category";

        private readonly List<SharkyDomainFilter> DomainFilters;

        /// <summary>
        /// In aggressive mode, the filter category will also block all navigation attempts to filtered URIs.
        /// </summary>
        public bool AggressiveMode = false;

        public SharkyFilterCategoryBase()
        {
            DomainFilters = [];
        }

        /// <summary>
        /// Add a domain filter into the category
        /// </summary>
        /// <param name="filter"></param>
        public void AddDomainFilter(SharkyDomainFilter filter)
        {
            DomainFilters.Add(filter);
        }

        /// <summary>
        /// Test a given URI against all filters in the category
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>TRUE if the URI is filtered, FALSE if not.</returns>
        public bool TestUri(Uri uri, SharkyFilteringContext context)
        {
            // If not in aggressive mode, only filter resource fetching, not navigation
            if (AggressiveMode == false && context == SharkyFilteringContext.Navigating)
            {
                return false;
            }

            // Test domain filters
            foreach (var filter in DomainFilters)
            {
                if (filter.TestDomain(uri.Host)) return true;
            }

            return false;
        }
    }

    public enum SharkyFilteringContext
    {
        Navigating,
        FetchingResource
    }
}
