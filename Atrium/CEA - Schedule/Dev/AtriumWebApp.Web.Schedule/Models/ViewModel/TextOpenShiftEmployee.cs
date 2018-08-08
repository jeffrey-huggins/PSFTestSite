using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class TextOpenShiftEmployee
    {
        public bool IsAdministrator { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public bool Selected { get; set; }
    }
}

