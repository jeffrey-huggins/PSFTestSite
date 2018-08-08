using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Maintenance.Models.ViewModels
{
    public class EquipmentManagementViewModel
    {
        public SelectList Communities { get; set; }
        public List<Equipment> Equipment { get; set; }
        public Equipment SelectedEquipment { get; set; }
    }
}
