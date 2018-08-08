using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class SOCFallIntervention
    {
        [Key]
        public int SOCFallInterventionId { get; set; }
        [Required]
        [MaxLength(30)]
        public string SOCFallInterventionName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}
