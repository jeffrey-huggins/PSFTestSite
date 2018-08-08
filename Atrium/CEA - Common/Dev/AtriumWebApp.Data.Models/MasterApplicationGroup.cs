using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    [Serializable]
    [DisplayColumn("ApplicationGroupName")]
    public class MasterApplicationGroup
    {
        [Key]
        public int ApplicationGroupId { get; set; }
        [StringLength(16)]
        [Required]
        public string ApplicationGroupCode { get; set; }
        [StringLength(32)]
        [Required]
        [DisplayName("Group Name")]
        public string ApplicationGroupName { get; set; }
        public int SortOrder { get; set; }

        public virtual IList<ApplicationInfo> ApplicationInfo { get; set; }
    }
}
