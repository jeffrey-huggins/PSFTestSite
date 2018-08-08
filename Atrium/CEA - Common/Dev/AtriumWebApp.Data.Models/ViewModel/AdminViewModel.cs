using System.Collections.Generic;

namespace AtriumWebApp.Models.ViewModel
{
    public class AdminViewModel
    {
        public int AppId { get; set; }
        public List<Community> Communities { get; set; }
        public List<ApplicationCommunityInfo> ApplicationCommunityInfos { get; set; }
        public List<Region> Regions { get; set; }
    }
}