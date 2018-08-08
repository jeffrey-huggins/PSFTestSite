using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class Hospital
    {
        public int Id { get; set; }
        [Required]
        [StringLength(60)]
        public string Name { get; set; }
        public bool AllowDataEntry { get; set; }
        public bool AllowReporting { get; set; }
        public int SortOrder { get; set; }
        public ICollection<Community> Communities { get; set; }
    }
}
