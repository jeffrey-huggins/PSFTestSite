using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class GeneralMeasureViewModel : BaseReviewMeasureViewModel
    {
        public bool RequiresPatientSample { get; set; }
    }
}