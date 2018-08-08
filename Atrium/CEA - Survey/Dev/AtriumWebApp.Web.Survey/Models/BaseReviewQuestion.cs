using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public abstract class BaseReviewQuestion
    {
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string Text { get; set; }
        public int MaxScore { get; set; }
        public int SortOrder { get; set; }
    }
}