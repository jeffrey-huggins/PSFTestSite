using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class SOCFallType
    {
        [Key]
        public int SOCFallTypeId { get; set; }
        [Required]
        [MaxLength(30)]
        public string SOCFallTypeName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}