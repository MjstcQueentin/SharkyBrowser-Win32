using SharkyBrowser.SharkyFilter.FilterCategories;
using SharkyBrowser.SharkySettings;
using System;
using System.Collections.Generic;

namespace SharkyBrowser.SharkyFilter
{
    /// <summary>
    /// This class is responsible for controlling the Sharky filter functionality.
    /// </summary>
    internal class SharkyFilterController
    {
        private static readonly List<SharkyFilterCategoryBase> FilterCategories = [];

        public static void Initialize()
        {
            FilterCategories.Clear();

            // Load filter categories based on user settings
            if (SharkyUserSettings.Instance.BlockAdultWebsites)
            {
                FilterCategories.Add(new SharkyAdultFilter());
            }

            if (SharkyUserSettings.Instance.BlockAdvertisements)
            {
                FilterCategories.Add(new SharkyAdvertisementFilter());
            }

            if (SharkyUserSettings.Instance.BlockBadware)
            {
                FilterCategories.Add(new SharkyMalwareFilter());
            }

            if (SharkyUserSettings.Instance.BlockAnnoyances)
            {
                FilterCategories.Add(new SharkyAnnoyanceFilter());
            }

            // Additional filter categories can be added here in the future.
        }

        /// <summary>
        /// Test a given URI against all active filter categories.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SharkyFilteredResource TestUri(Uri uri, SharkyFilteringContext context)
        {
            foreach (var category in FilterCategories)
            {
                if (category.TestUri(uri, context))
                {
                    return new()
                    {
                        HasMatched = true,
                        Uri = uri,
                        FilterName = category.ShortName
                    };
                }
            }

            return new()
            {
                HasMatched = false,
                Uri = uri,
                FilterName = "N/A"
            };
        }
    }

    public class SharkyFilteredResource
    {
        public bool HasMatched = false;
        public Uri Uri = new("http://localhost");
        public string FilterName = "N/A";
    }
}
