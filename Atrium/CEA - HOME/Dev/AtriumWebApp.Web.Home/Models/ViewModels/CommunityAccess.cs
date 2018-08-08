using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Home.Models.ViewModels
{
    [Serializable]
    public class CommunityAccess
    {
        public bool AppFlg { get; set; }
        public bool ReportFlg { get; set; }
        public int? CommunityId { get; set; }
        public int AppId { get; set; }
        public bool DeleteFlg { get; set; }
        public bool RowExists { get; set; }
    }
}