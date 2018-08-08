using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class SchdlSlotEditViewModel
    {
        public SchdlSlotEditViewModel()
        {
        }
        public string Title { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }
        public SchdlSlot Slot { get; set; }
        public Dictionary<DateTime, IList<SelectListItem>> StartTime { get; set; }
        public Dictionary<DateTime, IList<SelectListItem>> EndTime { get; set; }
        public Dictionary<DateTime, IList<SelectListItem>> RoomId { get; set; }
        public Dictionary<int, string> HourAltMap { get; set; }
    }
}