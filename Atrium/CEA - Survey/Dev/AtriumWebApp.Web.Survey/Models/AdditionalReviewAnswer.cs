using System;

namespace AtriumWebApp.Web.Survey.Models
{
    public class AdditionalReviewAnswer : BaseReviewAnswer
    {
        public int ReviewId { get; set; }
        public Review Review { get; set; }
        public int ReviewQuestionId { get; set; }
        public AdditionalReviewQuestion ReviewQuestion { get; set; }
    }
}