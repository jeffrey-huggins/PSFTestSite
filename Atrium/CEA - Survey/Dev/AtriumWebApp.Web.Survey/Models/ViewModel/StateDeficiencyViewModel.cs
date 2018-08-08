using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class StateDeficiencyViewModel
    {
        public StateDeficiency Deficiency { get; set; }
        public SelectList StateCodes { get; set; }
    }
}
