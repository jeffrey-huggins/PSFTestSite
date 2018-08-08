using System.Collections.Generic;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class MockSurveyFollowUpGroupViewModel
    {
        public string CommunityShortName { get; set; }
        public int MockSurveyId { get; set; }
        public int GroupId { get; set; }
        public string GroupType { get; set; }
        public string Description { get; set; }

        public string Title
        {
            get { return CommunityShortName + " - " + Description; }
        }

        public IList<CitationFollowUpViewModel> Citations { get; set; }
    }
}