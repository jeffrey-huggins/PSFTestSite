using System.Collections.Generic;

namespace AtriumWebApp.Web.Survey.Models
{
    public class GeneralMeasure : BaseReviewMeasure
    {
        public bool RequiresPatientSample { get; set; }

        public ICollection<GeneralQuestion> ReviewQuestions { get; set; }
    }
}