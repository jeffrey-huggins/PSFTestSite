using System.Collections.Generic;

namespace AtriumWebApp.Web.Survey.Models
{
    public class GeneralSection : BaseReviewSection
    {
        public int ReviewMeasureId { get; set; }
        public GeneralMeasure ReviewMeasure { get; set; }

        public ICollection<GeneralSample> ReviewSamples { get; set; }
        public ICollection<GeneralAnswer> ReviewAnswers { get; set; }
    }
}