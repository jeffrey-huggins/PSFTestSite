using System;

namespace AtriumWebApp.Web.Survey.Models
{
    public class GeneralQuestion : BaseReviewQuestion
    {
        public int ReviewMeasureId { get; set; }
        public GeneralMeasure ReviewMeasure { get; set; }
    }
}