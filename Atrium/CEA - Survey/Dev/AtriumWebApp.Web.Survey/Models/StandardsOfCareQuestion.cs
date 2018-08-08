using System;

namespace AtriumWebApp.Web.Survey.Models
{
    public class StandardsOfCareQuestion : BaseReviewQuestion
    {
        public int ReviewMeasureId { get; set; }
        public StandardsOfCareMeasure ReviewMeasure { get; set; }
    }
}