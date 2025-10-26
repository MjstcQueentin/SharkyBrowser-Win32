
namespace SharkyBrowser.SharkyFilter.FilterCategories
{
    /// <summary>
    /// This filter category should remove all adult content from the user's navigation.
    /// </summary>
    internal class SharkyAdultFilter : SharkyFilterCategoryBase
    {
        public SharkyAdultFilter()
        {
            AddDomainFilter(new FilterTypes.SharkyDomainFilter("", "http://sharky.lesmajesticiels.org/lists/domains/adult.txt"));

            // Enable aggressive mode to block navigation to adult sites as well
            AggressiveMode = true;
        }
    }
}
