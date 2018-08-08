using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models
{
    public abstract class BaseReviewSample
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string Comments { get; set; }

        public int ResidentId { get; set; }
        public Patient Resident { get; set; }
    }
}