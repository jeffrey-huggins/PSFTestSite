using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class TemplateSchdlSlotEditViewModel
    {
        public string Title { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }
        public TemplateSchdlSlot Slot { get; set; }
        public Dictionary<int, IList<SelectListItem>> StartTime { get; set; }
        public Dictionary<int, IList<SelectListItem>> EndTime { get; set; }
        public Dictionary<int, IList<SelectListItem>> RoomId { get; set; }
        public Dictionary<int, string> HourAltMap { get; set; }
    }
}