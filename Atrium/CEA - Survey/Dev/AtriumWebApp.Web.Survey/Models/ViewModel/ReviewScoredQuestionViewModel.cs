using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewScoredQuestionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int MaxPoints { get; set; }
        public int EarnedPoints { get; set; }
    }
}
