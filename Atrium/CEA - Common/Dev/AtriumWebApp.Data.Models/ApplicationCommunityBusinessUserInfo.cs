using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class ApplicationCommunityBusinessUserInfo
    {
        [Key, Column(Order=0),ForeignKey("ApplicationInfo")]
        public int ApplicationId { get; set; }
        [Key, Column(Order=1),ForeignKey("Community")]
        public int CommunityId { get; set; }
        [Key, Column(Order=2),ForeignKey("User")]
        public int BusinessUserId { get; set; }
        public bool ReportFlg { get; set; }
        public bool AppFlg { get; set; }

        public virtual ApplicationCommunityInfo AppCommInfo { get; set; }
        public virtual ApplicationInfo ApplicationInfo { get; set; }
        public virtual Community Community { get; set; }
        public virtual MasterBusinessUser User { get; set; }
    }
}
