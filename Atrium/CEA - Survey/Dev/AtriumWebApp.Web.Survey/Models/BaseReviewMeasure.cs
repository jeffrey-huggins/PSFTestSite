using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public abstract class BaseReviewMeasure
    {
        public int Id { get; set; }
        [Required]
        [StringLength(32)]
        public string Name { get; set; }
        [Required]
        [StringLength(32)]
        public string Code { get; set; }
        public int SortOrder { get; set; }
    }
}