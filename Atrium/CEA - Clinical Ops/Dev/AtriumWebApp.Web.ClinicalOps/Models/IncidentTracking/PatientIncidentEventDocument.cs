using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIncidentEventDocument
    {
        [Key]
        public int PatientIncidentEventDocumentId { get; set; }
        [Required]
        public int PatientIncidentEventId { get; set; }
        [Required]
        [MaxLength(256)]
        public string DocumentFileName { get; set; }
        [MaxLength(256)]
        public string ContentType { get; set; }
        public byte[] Document { get; set; }

        [ForeignKey("PatientIncidentEventId")]
        public virtual PatientIncidentEvent IncidentEvent { get; set; }
    }
}
