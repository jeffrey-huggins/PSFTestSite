using System;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class MockSurveyDeficiencyViewModel
    {
        public int DeficiencyId { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }

        public CitationViewModel Citation { get; set; }
    }
}