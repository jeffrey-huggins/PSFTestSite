using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class MasterBusinessUser
    {
        [Key]
        public int BusinessUserId { get; set; }
        public string DomainName { get; set; }
        public string AccountName { get; set; }
        public string ReportUserName { get; set; }
        public byte[] ADObjectGuid { get; set; }
        public string DisplayName { get; set; }

        public virtual List<ApplicationCommunityBusinessUserInfo> UserAccess { get; set; }
        public virtual List<SystemAppAdmin> AdminAccess { get; set; }
        public virtual List<SystemObjectPermissionRef> SpecialAccess { get; set; }
        public virtual List<SystemSysAdmin> SystemAdmin { get; set; }
    }
}
