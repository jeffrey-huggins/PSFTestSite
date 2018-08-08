using System;

namespace AtriumWebApp.Web.Survey.Models
{
    public class GeneralAnswer : BaseReviewAnswer
    {
        public int ReviewSectionId { get; set; }
        public GeneralSection ReviewSection { get; set; }
        public int ReviewQuestionId { get; set; }
        public GeneralQuestion ReviewQuestion { get; set; }
    }
}