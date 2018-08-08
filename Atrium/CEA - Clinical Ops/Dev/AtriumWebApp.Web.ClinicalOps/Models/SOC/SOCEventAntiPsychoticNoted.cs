using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class SOCEventAntiPsychoticNoted
    {
        [Key]
        public int SOCEventAntiPsychoticNotedId { get; set; }
        public int SOCEventId { get; set; }
		[Required]
        public DateTime NotedDate { get; set; }
        public DateTime? ReducedDate { get; set; }
		[MaxLength(256)]
        [Required]
        public string RecommendationDesc { get; set; }

        public DateTime? LastAimsTestDate { get; set; }
		[MaxLength(256)]
        public String TargetedBehaviors { get; set; }

        public SOCEventAntiPsychotic SOCEvent { get; set; }
    }
}
