using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.ViewModel
{
    [Serializable]
    public class Navigation
    {
        public Navigation()
        {
            Application = new List<ApplicationInfo>();
            Groups = new List<MasterApplicationGroup>();
            OtherTabs = new List<OtherNavTabs>();
        }
        public List<ApplicationInfo> Application { get; set; }
        public List<MasterApplicationGroup> Groups { get; set; }
        public DateTime Updated { get; set; }
        public List<OtherNavTabs> OtherTabs { get; set; }
    }

    [Serializable]
    public class OtherNavTabs
    {
        public OtherNavTabs()
        {
            NavItems = new List<OtherNav>();
            SortOrder = 0;
        }
        public string TabName { get; set; }
        public List<OtherNav> NavItems { get; set; }
        public int SortOrder { get; set; }
    }

    [Serializable]
    public class OtherNav
    {
        public OtherNav()
        {
            SortOrder = 0;
        }
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }
    }

}
