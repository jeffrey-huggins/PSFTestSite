using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SystemObjectPermission
    {
        [Key]
        public int ObjectPermissionId { get; set; }
        public int ApplicationId { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectDesc { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual ApplicationInfo Application { get; set; }
        public virtual List<SystemObjectPermissionRef> UserPermissions { get; set; }
    }
}
