using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Web.ClinicalOps.Models;
namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class SOCWoundViewModel
    {
		public DocumentViewModel File { get; set; }
		public SOCEventWound Wound { get; set; }
		public string WoundType { get; set; }
    }
}
