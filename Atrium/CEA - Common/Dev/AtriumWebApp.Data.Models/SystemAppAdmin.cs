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
    public class SystemAppAdmin
    {
        [Key]
        public int AppAdminId { get; set; }
        public int ApplicationId { get; set; }
        public int BusinessUserId { get; set; }
        public bool AdminFlg { get; set; }

        [ForeignKey("BusinessUserId")]
        public virtual MasterBusinessUser User { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual ApplicationInfo AppInfo { get; set; }
    }
}
