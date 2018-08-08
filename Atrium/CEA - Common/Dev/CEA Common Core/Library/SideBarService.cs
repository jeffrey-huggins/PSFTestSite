using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Base.Library
{
	public static class SideBarService
	{
		public static SideBarViewModel InitSideBar(BaseController controller, string AppCode, SharedContext Context)
		{
			var facilities = controller.FilterCommunitiesByADGroups(AppCode);
			var application = Context.Applications.Single(app => app.ApplicationCode == AppCode);
			int lookbackDays = application.LookbackDays;
			DateTime lookbackDay = DateTime.Today.AddDays(-lookbackDays);

			var sidebar = new SideBarViewModel()
			{
				FacilityList = facilities.ToList(),
				SelectedCommunity = facilities.First(),
				ResidentList = GetPatients(facilities.First(), lookbackDay, Context).ToList(),
				LookbackDate = lookbackDay
			};

			return sidebar;
		}

		private static List<Patient> GetPatients(Community facility, DateTime lookbackDay, SharedContext Context)
		{
			var residentList = Context.Residents.Where(a => a.CommunityId == facility.CommunityId
					&& a.LastCensusDate >= lookbackDay).ToList();

			return residentList;
		}

		public static ResidentViewModel GetPatient(int patientId, BaseController controller, SharedContext Context)
		{
			ResidentViewModel vm = new ResidentViewModel();
			//Set Patient
			var selectedResident = Context.Residents.Include("Room").First(a => a.PatientId == patientId);
			vm.ResidentName = selectedResident.LastName + ", " + selectedResident.FirstName;
			vm.CommunityName = Context.Facilities.Find(selectedResident.CommunityId).CommunityShortName;
			vm.AdmitDate = selectedResident.AdmitDate.ToString("d");
			vm.LastCensusDate = selectedResident.LastCensusDate.ToString("d");
			vm.BirthDate = selectedResident.BirthDate.HasValue ? selectedResident.BirthDate.Value.ToString("d") : "";
			vm.RoomName = selectedResident.Room.RoomName;
			vm.AdmissionDiagnosis = "No Diagnosis";
			var admissionDiagnosis = Context.PatientDiagnoses.Include("ICD9").Where(a => a.PatientId == patientId && a.IsAdmissionFlg)
				.Select(diagnosis => new
				{
					ICDDiseaseName = diagnosis.ICD9.DiseaseName,
					DiagnosisDate = diagnosis.DiagnosisDate
				}).Union(
				Context.PatientDiagnosesICD10.Include("ICD10").Where(a => a.PatientId == patientId && a.IsAdmissionFlg)
				.Select(diagnosis => new
				{
					ICDDiseaseName = diagnosis.ICD10.DiseaseName,
					DiagnosisDate = diagnosis.DiagnosisDate
				})).OrderByDescending(a => a.DiagnosisDate).FirstOrDefault();
			if (admissionDiagnosis != null)
			{
				vm.AdmissionDiagnosis = admissionDiagnosis.ICDDiseaseName;
			}
			//Set Diagnosis
			vm.OtherDiagnosis = Context.PatientDiagnoses.Include("ICD9").Where(a => a.PatientId == patientId && !a.IsAdmissionFlg)
				.Select(diagnosis => new
				{
					ICDDiseaseName = diagnosis.ICD9.DiseaseName,
					DiagnosisDate = diagnosis.DiagnosisDate
				}).Union(
					Context.PatientDiagnosesICD10.Include("ICD10").Where(a => a.PatientId == patientId && !a.IsAdmissionFlg)
				.Select(diagnosis => new
				{
					ICDDiseaseName = diagnosis.ICD10.DiseaseName,
					DiagnosisDate = diagnosis.DiagnosisDate
				})).OrderByDescending(p => p.DiagnosisDate).Take(3).Select(a => a.ICDDiseaseName).ToList();
			return vm;
		}

		public static SideBarViewModel SetSideBar(SideBarViewModel vm, BaseController controller, SharedContext Context)
		{
			var patients = GetPatients(vm.SelectedCommunity, vm.LookbackDate, Context);
			vm.ResidentList = patients;

			return vm;
		}

        private static List<Employee> GetTerminatedEmployees(int communityId, DateTime lookbackDay, bool includeActive, SharedContext Context)
        {
            List<Employee> employees;
            if (includeActive)
            {
                employees = Context.Employees.Where(a => a.CommunityId == communityId
                    && (a.TerminationDate == null || a.TerminationDate >= lookbackDay)).OrderBy(a => a.LastName).ToList();
            }
            else
            {
                employees = Context.Employees.Where(a => a.CommunityId == communityId 
                    && a.TerminationDate.HasValue ? a.TerminationDate.Value >= lookbackDay : false).ToList();
            }
            return employees;
        }

        private static List<Employee> GetEmployees(int communityId, DateTime lookbackDay, bool includeTerminated, SharedContext Context)
        {
            List<Employee> employees;
            if (includeTerminated)
            {
                employees = Context.Employees.Where(a => a.CommunityId == communityId
                    && (a.TerminationDate == null || a.TerminationDate >= lookbackDay)).OrderBy(a => a.LastName).ToList();
            }
            else
            {
                employees = Context.Employees.Where(a => a.CommunityId == communityId
                    && a.TerminationDate == null
                    && (a.EmployeeStatus == "Active" || a.EmployeeStatus == "Leave of Absence")).OrderBy(a => a.LastName).ToList();
            }
            return employees;
        }

        public static EmployeeSidebarViewModel InitEmployeeSideBar(BaseController controller, string AppCode,SharedContext Context,bool termEmployees = false)
        {
            var facilities = controller.FilterCommunitiesByADGroups(AppCode);
            var application = Context.Applications.Single(app => app.ApplicationCode == AppCode);
            int lookbackDays = application.LookbackDays;
            DateTime lookbackDay = DateTime.Today.AddDays(-lookbackDays);
            List<Employee> employees;
            if (termEmployees)
            {
                employees = GetTerminatedEmployees(facilities.First().CommunityId, lookbackDay, false, Context);
            }
            else
            {
                employees = GetEmployees(facilities.First().CommunityId, lookbackDay, false, Context);
            }
            var sidebar = new EmployeeSidebarViewModel()
            {
                FacilityList = facilities,
                SelectedCommunity = facilities.First(),
                EmployeeList = employees,
                LookbackDate = lookbackDay,
                ForTerminatedEmployees = termEmployees
            };

            return sidebar;
        }

        public static EmployeeViewModel GetEmployee(int employeeId, BaseController controller, SharedContext Context)
        {
            var employee = Context.Employees.Find(employeeId);
            var community = Context.Facilities.Find(employee.CommunityId);
            return new EmployeeViewModel()
            {
                Employee = employee,
                Community = community
            };
        }

        public static EmployeeSidebarViewModel SetEmployeeSideBar(EmployeeSidebarViewModel vm, BaseController controller, SharedContext Context)
        {
            List<Employee> employees;
            if (vm.ForTerminatedEmployees)
            {
                employees = GetTerminatedEmployees(vm.SelectedCommunity.CommunityId, vm.LookbackDate, vm.ShowTerminatedEmployees, Context);
            }
            else
            {
                employees = GetEmployees(vm.SelectedCommunity.CommunityId, vm.LookbackDate, vm.ShowTerminatedEmployees, Context);
            }
            vm.EmployeeList = employees;
            return vm;
        }
	}
}
