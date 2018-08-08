using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Maintenance.Models.ViewModels
{
    public class EquipmentDetailsViewModel
    {
        public Equipment Equipment { get; set; }
        public List<EquipmentInspection> Inspections { get; set; }
        public List<EquipmentRepair> Repairs { get; set; }
        public List<EquipmentMaintenancePlan> Plans { get; set; }
        public int LookbackDays { get; set; }
    }
}
