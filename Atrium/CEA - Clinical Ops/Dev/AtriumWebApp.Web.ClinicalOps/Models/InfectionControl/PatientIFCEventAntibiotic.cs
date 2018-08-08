using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCEventAntibiotic
    {
        [Key]
        public int PatientIFCEventId { get; set; }
        public PatientIFCEvent PatientIFCEvent { get; set; }

        [Key]
        public int PatientIFCAntibioticId { get; set; }
        [ForeignKey("PatientIFCAntibioticId")]
        public virtual PatientIFCAntibiotic Antibiotic { get; set; }
    }
}