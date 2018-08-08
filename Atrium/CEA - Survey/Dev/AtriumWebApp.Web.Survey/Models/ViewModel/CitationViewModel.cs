using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class CitationViewModel : CitationIdentityViewModel
    {
        public int MockSurveyId { get; set; }
        [Required]
        public int? DeficiencyId { get; set; }
        [Required]
        [StringLength(16)]
        [ContainedInEnumerable("ValidSeverities", true)]
        public string MockSeverity { get; set; }
        [StringLength(2048)]
        public string FindingDetails { get; set; }

        public static IReadOnlyCollection<string> ValidSeverities
        {
            get { return BaseMockCitation.ValidSeverities; }
        }
    }
}