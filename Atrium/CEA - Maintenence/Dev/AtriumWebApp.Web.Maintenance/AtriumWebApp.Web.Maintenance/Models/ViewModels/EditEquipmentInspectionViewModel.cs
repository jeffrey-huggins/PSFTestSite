using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AtriumWebApp.Web.Maintenance.Models.ViewModels
{
    public class EditEquipmentInspectionViewModel
    {
        public EquipmentInspection Inspection { get; set; }
        public List<DocumentViewModel> Documents {get;set;}
    }
}
