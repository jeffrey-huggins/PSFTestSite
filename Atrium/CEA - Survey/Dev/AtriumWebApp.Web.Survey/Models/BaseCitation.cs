using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public abstract class BaseCitation
    {
        [Key]
        public int Id { get; set; }
        [StringLength(1024)]
        public string Comments { get; set; }

        [StringLength(256)]
        public string SurveyCycleId { get; set; }
        public int SurveyId { get; set; }
        public CommunitySurvey Survey { get; set; }
    }
}