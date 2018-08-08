using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SOCEventAntiPsychoticNoted
    {
        [Key]
        public int SOCEventAntiPsychoticNotedId { get; set; }
        public int SOCEventId { get; set; }
        public DateTime NotedDate { get; set; }
        //public bool ReducedFlg { get; set; }
        public DateTime? ReducedDate { get; set; }
        //public bool ReviewedFlg { get; set; }
        //public DateTime? ReviewedDate { get; set; }
        //public bool QrtrlyAssessmentFlg { get; set; }
        public string RecommendationDesc { get; set; }

        public DateTime? LastAimsTestDate { get; set; }
        public String TargetedBehaviors { get; set; }

        public SOCEventAntiPsychotic SOCEvent { get; set; }
    }
}
