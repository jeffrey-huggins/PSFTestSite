using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models
{
    public abstract class BaseMockCitation
    {
        private static readonly IReadOnlyCollection<string> _ValidSeverities = new ReadOnlyCollection<string>(new[] { "Finding", "Concern", "Serious Concern", "Comments" });

        public int Id { get; set; }
        [Required]
        [StringLength(16)]
        [ContainedInEnumerable("ValidSeverities", true)]
        public string MockSeverity { get; set; }
        [StringLength(2048)]
        public string CitationComments { get; set; }
        [StringLength(2048)]
        public string PlanOfCorrection { get; set; }
        [StringLength(2048)]
        public string FollowUpComments { get; set; }
        public bool IsCompliant { get; set; }

        public int MockSurveyId { get; set; }
        public MockSurvey MockSurvey { get; set; }

        public int DeficiencyId { get; set; }
        public BaseDeficiency BaseDeficiency { get; set; }

        public static IReadOnlyCollection<string> ValidSeverities
        {
            get { return _ValidSeverities; }
        }
    }
}