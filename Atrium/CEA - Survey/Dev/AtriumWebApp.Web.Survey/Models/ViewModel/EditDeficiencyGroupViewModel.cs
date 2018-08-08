using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class EditDeficiencyGroupViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(64)]
        public string Description { get; set; }
        public int SortOrder { get; set; }
    }
}