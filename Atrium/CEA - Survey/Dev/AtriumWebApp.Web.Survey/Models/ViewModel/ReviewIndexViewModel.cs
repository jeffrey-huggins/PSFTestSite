using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewIndexViewModel
    {
        public bool CanCloseDietary { get; set; }
        public bool CanCloseNursing { get; set; }
        public bool CanEditDate { get; set; }
        public bool CanEdiit { get; set; }
        public bool CanDelete { get; set; }
        public IList<Review> Items { get; set; }
        [DisplayName("Community")]
        public int CurrentCommunity { get; set; }
        public IList<Community> Communities { get; set; }
        [DisplayName("From")]
        public DateTime OccurredRangeFrom { get; set; }
        [DisplayName("To")]
        public DateTime OccurredRangeTo { get; set; }
        public ReviewViewModel Review { get; set; }
    }
}
