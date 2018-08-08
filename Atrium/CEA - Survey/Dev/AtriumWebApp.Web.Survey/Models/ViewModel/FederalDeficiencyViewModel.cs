using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class FederalDeficiencyViewModel
    {
        public FederalDeficiency Deficiency { get; set; }
        public SelectList SurveyPayerGroups { get; set; }
    }
}
