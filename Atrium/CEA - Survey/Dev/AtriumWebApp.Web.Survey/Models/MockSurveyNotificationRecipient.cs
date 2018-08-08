using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public class MockSurveyNotificationRecipient
    {
        public int Id { get; set; }
        public int CommunityId { get; set; }
        [EmailAddress]
        [StringLength(256)]
        public string EmailAddress { get; set; }
    }
}