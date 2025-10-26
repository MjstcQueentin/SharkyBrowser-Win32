
namespace SharkyBrowser.SharkyFilter.FilterCategories
{
    internal class SharkyAnnoyanceFilter : SharkyFilterCategoryBase
    {
        public SharkyAnnoyanceFilter()
        {
            AddDomainFilter(new FilterTypes.SharkyDomainFilter("", "http://sharky.lesmajesticiels.org/lists/domains/annoyances.txt"));
        }
    }
}
