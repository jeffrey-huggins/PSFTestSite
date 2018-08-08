using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewAnswerViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCompliant { get; set; }
    }
}
