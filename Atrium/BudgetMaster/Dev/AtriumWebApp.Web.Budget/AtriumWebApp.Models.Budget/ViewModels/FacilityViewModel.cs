using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.Budget.ViewModels
{
    public class FacilityViewModel
    {
        public List<Facility> Facilities { get; set; }
        public List<State> States { get; set; }
        public List<Region> Regions { get; set; }
    }
}
