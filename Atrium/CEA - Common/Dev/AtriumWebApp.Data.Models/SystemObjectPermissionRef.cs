using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SystemObjectPermissionRef
    {
        [Key, Column(Order=0)]
        public int ObjectPermissionId { get; set; }
        [Key, Column(Order = 1)]
        public int CommunityId { get; set; }
        [Key, Column(Order=2)]
        public int BusinessUserId { get; set; }
        [Key, Column(Order = 3)]
        public int ApplicationId { get; set; }
        public bool EnabledFlg { get; set; }

        [ForeignKey("BusinessUserId")]
        public virtual MasterBusinessUser User { get; set; }
        [ForeignKey("ObjectPermissionId")]
        public virtual SystemObjectPermission ObjectPermissions { get; set; }
        //[ForeignKey("CommunityId, BusinessUserId, ApplicationId")]
        //public virtual ApplicationCommunityBusinessUserInfo UserAccess { get; set; }

    }
}
