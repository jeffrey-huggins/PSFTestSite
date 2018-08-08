using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class EmployeeIFCEvent
    {
        [Key]
        public int EmployeeIFCEventId { get; set; }
        public int EmployeeId { get; set; }
        [DisplayName("Onset Date")]
        public DateTime OnsetDate { get; set; }
        [DisplayName("Work Days Missed")]
        public int MissedWorkDaysCnt { get; set; }
        [DisplayName("Site")]
        public int PatientIFCSiteId { get; set; }
        public bool DeletedFlg { get; set; }
        public DateTime? DeletedTS { get; set; }
        public string DeletedADDomainName { get; set; }
        [ForeignKey("PatientIFCSiteId")]
        public virtual PatientIFCSite Site { get; set; }
    }
}