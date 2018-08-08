using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class Region
    {
        [Key]
        public int RegionId { get; set; }
        public string RegionName { get; set; }
    }
}