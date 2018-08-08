using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public abstract class BaseReviewSection
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string Comments { get; set; }

        public int ReviewId { get; set; }
        public Review Review { get; set; }
    }
}