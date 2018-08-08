using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class CitationPlanOfCorrectionViewModel
    {
        public int CitationId { get; set; }
        public string FindingDetails { get; set; }
        [DisplayName("Plan of Correction (2048 max)")]
        [StringLength(2048)]
        public string PlanOfCorrection { get; set; }
        public string MockSeverity { get; set; }
        public string DeficiencyDescription { get; set; }
        public string DeficiencyInstructions { get; set; }
    }
}
