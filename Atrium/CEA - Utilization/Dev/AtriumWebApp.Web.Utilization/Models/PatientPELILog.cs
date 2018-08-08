using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PatientPELILog
    {
        [Key]
        public int PELILogId { get; set; }
        public int PatientId { get; set; }

        public int PELITypeId { get; set; }
        public virtual IEnumerable<PELIType> PELIType { get; set; }

        public DateTime? AdmitDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool DeletedFlg { get; set; }

        public DateTime InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }
}