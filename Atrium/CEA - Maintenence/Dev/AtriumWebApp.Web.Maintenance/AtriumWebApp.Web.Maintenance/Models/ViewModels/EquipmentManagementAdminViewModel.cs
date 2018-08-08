using AtriumWebApp.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Maintenance.Models.ViewModels
{
    public class EquipmentManagementAdminViewModel
    {
        public AdminViewModel AdminViewModel { get; set; }
        public int LookbackDays { get; set; }
    }
}
