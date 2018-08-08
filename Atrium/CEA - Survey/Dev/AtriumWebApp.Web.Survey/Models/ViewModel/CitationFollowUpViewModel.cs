using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class CitationFollowUpViewModel
    {
        public int CitationId { get; set; }
        public string FindingDetails { get; set; }
        public string PlanOfCorrection { get; set; }
        [DisplayName("Follow-up Details (2048 max)")]
        [StringLength(2048)]
        public string FollowUpDetails { get; set; }
        [DisplayName("Compliant?")]
        public bool IsCompliant { get; set; }
        public string MockSeverity { get; set; }
        public string DeficiencyDescription { get; set; }
        public string DeficiencyInstructions { get; set; }
    }
}