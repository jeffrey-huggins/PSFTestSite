using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCEventReCulture
    {
        [Key]
        public int PatientIFCEventId { get; set; }
        public PatientIFCEvent PatientIFCEvent { get; set; }

        [Key]
        public int ReCultureId { get; set; }
        public DateTime RecultureDate { get; set; }
    }
}