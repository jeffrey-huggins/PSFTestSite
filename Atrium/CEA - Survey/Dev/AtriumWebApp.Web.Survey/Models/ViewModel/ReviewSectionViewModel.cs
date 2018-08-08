using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewSectionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ReviewSectionType SectionType { get; set; }
        [StringLength(128)]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
        public bool RequiresSample { get; set; }
        public bool HasSample { get; set; }

        public IList<ReviewScoredQuestionViewModel> Questions { get; set; }

        public int EarnedPoints
        {
            get
            {
                if (Questions == null)
                {
                    return 0;
                }
                return Questions.Sum(q => q.EarnedPoints);
            }
        }

        public int PossiblePoints
        {
            get
            {
                if (Questions == null)
                {
                    return 0;
                }
                return Questions.Sum(q => q.MaxPoints);
            }
        }
    }
}