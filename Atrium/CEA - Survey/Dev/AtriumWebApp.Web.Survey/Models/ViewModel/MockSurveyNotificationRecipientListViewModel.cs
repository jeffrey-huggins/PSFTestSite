using System.Collections.Generic;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class MockSurveyNotificationRecipientListViewModel
    {
        public int CommunityId { get; set; }
        public string CommunityShortName { get; set; }
        public IList<MockSurveyNotificationRecipient> Items { get; set; }
    }
}