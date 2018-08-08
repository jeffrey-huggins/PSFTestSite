using System;

namespace AtriumWebApp.Web.Survey.Models
{
    public abstract class BaseReviewAnswer
    {
        public int Id { get; set; }
        public bool IsCompliant { get; set; }
        public int EarnedScore { get; set; }
    }
}