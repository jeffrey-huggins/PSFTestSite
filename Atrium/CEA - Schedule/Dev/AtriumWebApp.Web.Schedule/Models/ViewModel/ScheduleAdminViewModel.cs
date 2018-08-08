using System.Collections.Generic;
using System.ComponentModel;
using AtriumWebApp.Models.ViewModel;
namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class ScheduleAdminViewModel
    {
        public bool IsAdministrator { get; set; }
        public AdminViewModel AdminViewModel { get; set; }
        public IEnumerable<MasterAtriumPatientGroup> AreaRooms { get; set; }
    }
}