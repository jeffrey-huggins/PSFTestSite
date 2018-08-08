using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class NotificationRecipientViewModel
    {
        public int CommunityId { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}