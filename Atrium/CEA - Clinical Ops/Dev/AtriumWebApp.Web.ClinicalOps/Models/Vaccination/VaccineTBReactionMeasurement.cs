using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class VaccineTBReactionMeasurement
    {
        [Key]
        public int VaccineTBReactionMeasurementId { get; set; }
        public string Description { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}