using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCEventTypeOfPrecaution
    {
        [Key]
        public int PatientIFCEventId { get; set; }
        public PatientIFCEvent PatientIFCEvent { get; set; }

        [Key]
        public int PatientIFCTypeOfPrecautionId { get; set; }
        [ForeignKey("PatientIFCTypeOfPrecautionId")]
        public virtual PatientIFCTypeOfPrecaution Precaution { get; set; }
    }
}