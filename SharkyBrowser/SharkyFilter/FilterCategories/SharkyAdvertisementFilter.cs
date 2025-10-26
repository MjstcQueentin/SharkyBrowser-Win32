
namespace SharkyBrowser.SharkyFilter.FilterCategories
{
    internal class SharkyAdvertisementFilter : SharkyFilterCategoryBase
    {
        public SharkyAdvertisementFilter()
        {
            Name = "Advertisement Filter";
            AddDomainFilter(new("", "http://sharky.lesmajesticiels.org/lists/domains/ads.txt"));
            AddDomainFilter(new("", "https://gist.githubusercontent.com/HiddenMotives/b627aa1a0baa90cbca6a/raw/49179ce11e4530ab13557ea9960689e3395fadf5/ad_websites.txt"));
        }
    }
}
