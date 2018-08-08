using System;

namespace AtriumWebApp.Web.Survey.Models
{
    public class StandardsOfCareAnswer : BaseReviewAnswer
    {
        public int ReviewSampleId { get; set; }
        public StandardsOfCareSample ReviewSample { get; set; }
        public int ReviewQuestionId { get; set; }
        public StandardsOfCareQuestion ReviewQuestion { get; set; }
    }
}