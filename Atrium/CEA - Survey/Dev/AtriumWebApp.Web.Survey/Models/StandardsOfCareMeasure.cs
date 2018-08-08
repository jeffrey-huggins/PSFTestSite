using System.Collections.Generic;

namespace AtriumWebApp.Web.Survey.Models
{
    public class StandardsOfCareMeasure : BaseReviewMeasure
    {
        public int ThresholdBonusScore { get; set; }

        public ICollection<StandardsOfCareQuestion> ReviewQuestions { get; set; }
    }
}