using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SOCAntiPsychoticMedication
    {
        [Key]
        public int AntiPsychoticMedicationId { get; set; }
        public string AntiPsychoticMedicationName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}
