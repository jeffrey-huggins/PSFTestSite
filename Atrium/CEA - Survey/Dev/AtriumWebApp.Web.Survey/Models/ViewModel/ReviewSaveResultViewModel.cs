using AtriumWebApp.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewSaveResultViewModel : SaveResultViewModel
    {
        public string ReviewDate { get; set; }
        public bool IsClosed { get; set; }
        public string FormattedNursingClosedDate { get; set; }
        public string FormattedDietaryClosedDate { get; set; }
        public bool CanCloseDietary { get; set; }
        public bool CanCloseNursing { get; set; }
        public bool CanEditDate { get; set; }
        public bool CanEdiit { get; set; }
        public bool CanDelete { get; set; }
    }
}