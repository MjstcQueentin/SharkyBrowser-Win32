
namespace SharkyBrowser.SharkyFilter.FilterCategories
{
    internal class SharkyAnnoyanceFilter : SharkyFilterCategoryBase
    {
        public SharkyAnnoyanceFilter()
        {
            Name = "Annoyance Filter";
            AddDomainFilter(new FilterTypes.SharkyDomainFilter("", "http://sharky.lesmajesticiels.org/lists/domains/annoyances.txt"));
        }
    }
}
