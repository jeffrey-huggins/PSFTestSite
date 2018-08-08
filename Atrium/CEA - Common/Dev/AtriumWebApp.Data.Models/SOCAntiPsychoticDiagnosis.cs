using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SOCAntiPsychoticDiagnosis
    {
        [Key]
        public int AntiPsychoticDiagnosisId { get; set; }
        [Required]
        [MaxLength(30)]
        public string AntiPsychoticDiagnosisDesc { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }


    }
}
