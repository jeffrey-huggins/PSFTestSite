using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCEventOrganism
    {
        [Key]
        public int PatientIFCEventId { get; set; }
        public PatientIFCEvent PatientIFCEvent { get; set; }

        [Key]
        public int PatientIFCOrganismId { get; set; }
        [ForeignKey("PatientIFCOrganismId")]
        public virtual PatientIFCOrganism Organism { get; set; }
    }
}