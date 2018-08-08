using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Maintenance.Models.ViewModels
{
    public class EditEquipmentRepairViewModel
    {
        public EquipmentRepair Repair { get; set; }
        public List<DocumentViewModel> Documents { get; set; }
    }
}
