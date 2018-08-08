using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class SystemSysAdmin
    {
        [Key]
        public int SysAdminId { get; set; }
        public int BusinessUserId { get; set; }
        public bool AdminFlg { get; set; }

        [ForeignKey("BusinessUserId")]
        public virtual MasterBusinessUser User { get; set; }
    }
}
