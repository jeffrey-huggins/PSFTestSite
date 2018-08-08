using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewQuestionViewModel
    {
        public int Id { get; set; }
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }
        [StringLength(256)]
        public string Text { get; set; }
        public int MaxPoints { get; set; }
        public int SortOrder { get; set; }
    }
}