using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public class AdditionalReviewQuestion
    {
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string Text { get; set; }
        public int Score { get; set; }
        public int SortOrder { get; set; }
    }
}