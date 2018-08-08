using AtriumWebApp.Models;
using System;
using System.Collections.Generic;

namespace AtriumWebApp.Models.ViewModel
{
    public class EmployeeSidebarViewModel
    {
        public bool ForTerminatedEmployees { get; set; }
        public bool ShowTerminatedEmployees { get; set; }
        public DateTime LookbackDate { get; set; }
        public List<Community> FacilityList { get; set; }
        public List<Employee> EmployeeList { get; set; }
        public Employee SelectedEmployee { get; set; }
        public Community SelectedCommunity { get; set; }
        public string RedirectUrl { get; set; }

        public string AppCode { get; set; }
        public string AppController { get; set; }
        public string ChangeCommunityPath { get; set; }
        public string SideBarDDLActionName { get; set; }
    }
}