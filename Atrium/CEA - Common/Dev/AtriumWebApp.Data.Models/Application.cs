using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class ApplicationInfo
    {
        [Key]
        public int ApplicationId { get; set; }
        [DisplayName("App Code")]
        public string ApplicationCode { get; set; }
        [DisplayName("Display Name")]
        public string ApplicationName { get; set; }
        public int LookbackDays { get; set; }
        [DisplayName("Group Id")]
        public int ApplicationGroupId { get; set; }
        [DisplayName("Relative URL")]
        public string RelativeApplicationURL { get; set; }
        [DisplayName("Admin URL")]
        public string RelativeAdminURL { get; set; }
        public int SortOrder { get; set; }
        public bool EnabledFlg { get; set; }

        [ForeignKey("ApplicationGroupId")]
        public virtual MasterApplicationGroup MasterApplicationGroup { get; set; }

        public virtual List<ApplicationCommunityBusinessUserInfo> UserAccessList { get; set; }

        public virtual List<SystemAppAdmin> UserAdminList { get; set; }

        public virtual List<ApplicationCommunityInfo> CommunityInfo { get; set; }

    }
}