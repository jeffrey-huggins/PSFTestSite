using System.Collections.Generic;

namespace AtriumWebApp.Web.Survey.Models
{
    public class GeneralSample : BaseReviewSample
    {
        public int ReviewSectionId { get; set; }
        public GeneralSection ReviewSection { get; set; }

        public ICollection<GeneralPatientAnswer> ReviewAnswers { get; set; }
    }
}