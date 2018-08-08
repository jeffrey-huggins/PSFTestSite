using AtriumWebApp.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.HR.Models.ViewModel
{
    public class STNATrainingAdminViewModel
    {
        public IList<STNATrainingFacility> TrainingFacilities { get; set; }
        //public IList<STNATrainingActionItem> TrainingActionItems { get; set; }
        public STNATrainingFacility NewFacility { get { return new STNATrainingFacility(); } }
        public AdminViewModel AdminViewModel { get; set; }
    }
}