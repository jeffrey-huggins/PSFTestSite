using System;

namespace AtriumWebApp.Web.Survey.Models
{
    public class GeneralPatientAnswer : BaseReviewAnswer
    {
        public int ReviewSampleId { get; set; }
        public GeneralSample ReviewSample { get; set; }
        public int ReviewQuestionId { get; set; }
        public GeneralQuestion ReviewQuestion { get; set; }
    }
}