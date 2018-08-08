using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.ViewModel
{
    public class CareConferenceAttendanceViewModel
    {
        public List<CareConferenceAttendance> Attendances { get; set; }
        public List<Employee> CNAs { get; set; }
    }
}
