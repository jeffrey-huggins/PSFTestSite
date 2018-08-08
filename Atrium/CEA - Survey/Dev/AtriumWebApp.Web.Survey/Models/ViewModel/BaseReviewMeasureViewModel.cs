using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public abstract class BaseReviewMeasureViewModel
    {
        public int Id { get; set; }
        [StringLength(32)]
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }
}