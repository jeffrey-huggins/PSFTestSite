using System.Collections.Generic;

namespace AtriumWebApp.Web.Survey.Models
{
    public class StandardsOfCareSection : BaseReviewSection
    {
        public int ReviewMeasureId { get; set; }
        public StandardsOfCareMeasure ReviewMeasure { get; set; }

        public ICollection<StandardsOfCareSample> ReviewSamples { get; set; }
    }
}