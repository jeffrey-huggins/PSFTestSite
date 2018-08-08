using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Schedule.Managers;
using AtriumWebApp.Web.Schedule.Models;
using AtriumWebApp.Web.Schedule.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Schedule.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "SCH")]
    public class ScheduleController : ScheduleBaseController
    {
        public ScheduleController(IOptions<AppSettingsConfig> config, ScheduleContext context) : base(config, context)
        {

        }

        protected new ScheduleContext Context
        {
            get { return (ScheduleContext)base.Context; }
        }

        private const string AppCode = "SCH";
        private const string PayerGroupCode = "SNF";
        private IDictionary<string, bool> _AdminAccess;
        private ScheduleLogManager LogManager = new ScheduleLogManager();
        private IDictionary<string, bool> UserAccess
        {
            get
            {
                Session.TryGetObject("userAccess", out IDictionary<string, bool> access);
                return access;
            }
            set { Session.SetItem("userAccess", value); }
        }


        public ActionResult Index()
        {
            if (UserAccess == null)
            {
                var appCodes = new List<string>();

                // "ADR", "POR", "CON", 
                appCodes.AddRange(new List<string> { "SCH" });

                UserAccess = DetermineUserAccess(PrincipalContext, UserPrincipal, appCodes);
            }

            LogSession(AppCode);
            SetDateRangeErrorValues();
            SetLookbackDays(HttpContext, AppCode);
            SetInitialTableRangeLookback(AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);

            PayPeriodScheduleViewModel viewModel = new PayPeriodScheduleViewModel();
            viewModel.CurrentCommunity = null;
            viewModel.Communities = GetCommunitiesListItems();
            viewModel.CanDelete = DetermineObjectAccess("0001", null, AppCode);
            viewModel.CanManageTemplates = DetermineObjectAccess("0002", null, AppCode);
            viewModel.IsDisabled = true;

            return View(viewModel);
        }
        [HttpPost]
        public JsonResult CopyScheduleFromMaster(int communityId, string dateOfWeek)
        {
            if (communityId < 1)
                return Json(new { success = false, responseText = "Community < 1" });
            var payPeriodDate = PayPeriodScheduleViewModel.ParseFirstDateofWeek(dateOfWeek);

            if (ScheduleForWeekExist(communityId, payPeriodDate))
            {
                return Json(new { success = false, responseText = "Pay period already exists." });
            }
            if (Context.TemplatePayPeriod.Where(a => a.CommunityId == communityId).Count() == 0)
            {
                return Json(new
                {
                    success = false,
                    responseText = "Master schedule does not exist.  Contact the corporate scheduler to get one created."
                });
            }
            Context.TemplatePayPeriod.Include(a => a.PayerGroups.Select(
                b => b.ScheduleLedger.Select(
                    c => c.Slots.Select(
                        d => d.Employee.JobClasses.Select(
                            e => e.GLAccount))))).Where(a => a.CommunityId == communityId).FirstOrDefault().PayerGroups.ToList()
                .ForEach(a => a.ScheduleLedger.ToList()
                .ForEach(b => b.Slots.Where(c => (c.Employee != null ? c.Employee.EmployeeStatus != "Active" : false) 
                || (c.Employee != null ? !CodeIsValid(c.Employee.JobClasses.ToList(), b.GeneralLedger, payPeriodDate):false)).ToList()
                .ForEach(c =>
                {
                    c.EmployeeId = null;
                    c.SchdlSlotAltId = Context.AltSlots.Where(d => d.SlotAltCode == "Unass").FirstOrDefault().Id;
                })));
            Context.SaveChanges();

            TemplateSchdlPayPeriod templatePayPeriod = Context.TemplatePayPeriod.Where(a => a.CommunityId == communityId).FirstOrDefault();
            SchdlPayPeriod payPeriod = new SchdlPayPeriod()
            {
                CommunityId = communityId,
                PayPeriodBeginDate = payPeriodDate,
                PayerGroups = new List<SchdlPayerGroup>()
            };

            foreach (TemplateSchdlPayerGroup templateGroup in templatePayPeriod.PayerGroups)
            {
                SchdlPayerGroup payerGroup = new SchdlPayerGroup()
                {
                    AtriumPayerGroupCode = templateGroup.AtriumPayerGroupCode,
                    AvgDailyCensusCnt = templateGroup.AvgDailyCensusCnt,
                    PayPeriod = payPeriod,
                    ScheduleLedger = new List<SchdlGeneralLedger>()
                };
                payPeriod.PayerGroups.Add(payerGroup);
                foreach (TemplateSchdlGeneralLedger templateLedger in templateGroup.ScheduleLedger)
                {
                    SchdlGeneralLedger ledger = new SchdlGeneralLedger()
                    {
                        GeneralLedgerId = templateLedger.GeneralLedgerId,
                        HourPPDCnt = templateLedger.HourPPDCnt,
                        Slots = new List<SchdlSlot>(),
                        PayerGroup = payerGroup
                    };
                    payerGroup.ScheduleLedger.Add(ledger);
                    foreach (TemplateSchdlSlot templateSlot in templateLedger.Slots)
                    {
                        SchdlSlot slot = new SchdlSlot()
                        {
                            WorkShiftId = templateSlot.WorkShiftId,
                            SlotNbr = templateSlot.SlotNbr,
                            EmployeeId = templateSlot.EmployeeId,
                            SchdlSlotAltId = templateSlot.SchdlSlotAltId,
                            Ledger = ledger,
                            Days = new List<SchdlSlotDay>()
                        };
                        ledger.Slots.Add(slot);
                        foreach (TemplateSchdlSlotDay templateDay in templateSlot.Days)
                        {
                            SchdlSlotDay day = new SchdlSlotDay()
                            {
                                WorkDate = payPeriodDate.AddDays(templateDay.PayPeriodDayNbr - 1),
                                AtriumPatientGroupId = templateDay.AtriumPatientGroupId,
                                ShiftStartTime = templateDay.ShiftStartTime,
                                ShiftEndTime = templateDay.ShiftEndTime,
                                HourCnt = templateDay.HourCnt,
                                SchdlHourAltId = templateDay.SchdlHourAltId,
                                Slot = slot
                            };
                            slot.Days.Add(day);
                        }
                    }
                }
            }
            Context.Week.Add(payPeriod);
            Context.SaveChanges();
            return Json(new
            {
                success = true
            });

        }

        [HttpPost]
        public JsonResult DoesScheduleForWeekExist(int communityId, string payPeriod)
        {
            if (communityId < 1)
                return new JsonResult(new { });
            var payPeriodDate = PayPeriodScheduleViewModel.ParseFirstDateofWeek(payPeriod);
            bool scheduleExist = ScheduleForWeekExist(communityId, payPeriodDate);
            if (scheduleExist || !(Context.TemplatePayPeriod.Where(a => a.CommunityId == communityId).Count() == 0))
            {
                LogCreateEditSchedule(communityId, payPeriodDate);
            }

            return Json(new { success = true, ScheduleWeekExist = scheduleExist });
        }

        private Boolean ScheduleForWeekExist(int? communityId, DateTime payPeriod)
        {
            var weeklySchedule =
            (from week in Context.Week
             where week.PayPeriodBeginDate == payPeriod
                 && week.CommunityId == communityId
             select week).Include(a => a.PayerGroups.Select(
                b => b.ScheduleLedger.Select(
                    c => c.Slots.Select(
                        d => d.Employee.JobClasses.Select(
                            e => e.GLAccount)))));
            if (DateTime.Now < payPeriod.AddDays(14) && weeklySchedule.Count() > 0)
            {
                weeklySchedule.FirstOrDefault().PayerGroups.ToList()
                    .ForEach(a => a.ScheduleLedger.ToList()
                    .ForEach(b => b.Slots.Where(c => (c.Employee != null ? c.Employee.EmployeeStatus != "Active" : false) || (c.Employee != null ? !CodeIsValid(c.Employee.JobClasses.ToList(), b.GeneralLedger, payPeriod) :false)).ToList()
                    .ForEach(c =>
                {
                    c.Employee = null;
                    c.EmployeeId = null;
                    c.SchdlSlotAltId = Context.AltSlots.Where(d => d.SlotAltCode == "Unass").FirstOrDefault().Id;
                    c.SchdlSlotAlt = Context.AltSlots.Where(d => d.SlotAltCode == "Unass").FirstOrDefault();
                })));
                Context.SaveChanges();
            }
            return (weeklySchedule.Count() > 0);

        }

        private bool IsAdministrator
        {
            get
            {
                if (_AdminAccess == null)
                {
                    _AdminAccess = DetermineAdminAccess(PrincipalContext, UserPrincipal);
                }
                bool isAdministrator;
                if (_AdminAccess.TryGetValue(AppCode, out isAdministrator))
                {
                    return isAdministrator;
                }
                return false;
            }
        }


        [HttpPost]
        public ActionResult EditSlot(string slotId)
        {
            if (!int.TryParse(slotId, out int id))
            {
                return View();
            }

            SchdlSlot slot = Context.Slots.Find(id);

            int communityID = slot.Ledger.PayerGroup.PayPeriod.CommunityId;
            var workDate = slot.Days.First().WorkDate;
            var jobTypes = MapCodes(slot.Ledger.GeneralLedger.AccountNbr, true);
            var employees = Context.EmployeeJobClasses.Where(a => (jobTypes.Contains(a.GLAccount.AccountNbr))
                    && a.StartDate <= workDate && a.StopDate >= workDate
                        && (a.Employee.CommunityId == communityID
                            && a.Employee.EmployeeStatus == "Active"))
                        .Select(a => a.Employee).ToList();


            //var employees = Context.Employees.Where(a => a.CommunityId == communityID && a.EmployeeStatus == "Active" 
            //    && jobTypes.Contains(a.));
            var altDaySlots = Context.AltSlots.Where(a => a.Id > 0);
            IList<SelectListItem> dropdownList = GetScheduleDropdownValues(altDaySlots.ToList(), employees.Distinct().ToList());
            if (slot.Employee != null)
            {
                dropdownList.FirstOrDefault(a => a.Value == slot.EmployeeId.ToString()).Selected = true;
            }
            else if (slot.SchdlSlotAlt != null)
            {
                dropdownList.FirstOrDefault(a => a.Value == (slot.SchdlSlotAltId * -1).ToString()).Selected = true;
            }
            Dictionary<DateTime, IList<SelectListItem>> startTime = new Dictionary<DateTime, IList<SelectListItem>>();
            Dictionary<DateTime, IList<SelectListItem>> endTime = new Dictionary<DateTime, IList<SelectListItem>>();
            Dictionary<DateTime, IList<SelectListItem>> roomId = new Dictionary<DateTime, IList<SelectListItem>>();
            List<MasterAtriumPatientGroup> rooms = Context.AreaRoom.Where(a => a.CommunityId == slot.Ledger.PayerGroup.PayPeriod.CommunityId).ToList();
            var altHourSlots = Context.AltHours.Where(a => a.Id > 0);
            foreach (SchdlSlotDay day in slot.Days)
            {
                IList<SelectListItem> room = GetCommunityRooms(rooms);
                IList<SelectListItem> shiftStart = GetHourDropdownValues(altHourSlots.ToList(), true);
                IList<SelectListItem> shiftEnd = GetHourDropdownValues(altHourSlots.ToList(), false);
                if (day.ShiftStartTime.HasValue)
                {
                    string timeValue = day.ShiftStartTime.Value.ToString("HHmm").ToLower();
                    shiftStart.FirstOrDefault(a => a.Value == timeValue).Selected = true;
                }
                if (day.ShiftEndTime.HasValue)
                {
                    string timeValue = day.ShiftEndTime.Value.ToString("HHmm").ToLower();
                    shiftEnd.FirstOrDefault(a => a.Value == timeValue).Selected = true;
                }
                if (day.SchdlHourAltId.HasValue)
                {
                    shiftStart.FirstOrDefault(a => a.Value == (-1 * day.SchdlHourAltId.Value).ToString()).Selected = true;
                }
                if (day.AtriumPatientGroupId.HasValue)
                {
                    room.FirstOrDefault(a => a.Value == day.AtriumPatientGroupId.Value.ToString()).Selected = true;
                }
                startTime.Add(day.WorkDate, shiftStart);
                endTime.Add(day.WorkDate, shiftEnd);
                roomId.Add(day.WorkDate, room);
            }

            SchdlSlotEditViewModel editSlot = new SchdlSlotEditViewModel
            {
                Title = MapCodes(slot.Ledger.GeneralLedger.AccountNbr)[0] + " Edit Employees:",
                Slot = slot,
                EmployeeList = dropdownList,
                StartTime = startTime,
                EndTime = endTime,
                RoomId = roomId,
                HourAltMap = GetAltHourMapping()
            };
            return PartialView("EditSlot", editSlot);

        }

        public Dictionary<DateTime, string> GetValuesFromString(string input)
        {
            Dictionary<DateTime, string> values = new Dictionary<DateTime, string>();
            foreach (string value in input.TrimEnd(',').Split(','))
            {
                DateTime date = DateTime.Parse(value.Split(';')[0]);
                values.Add(date, value.Split(';')[1]);
            }
            return values;
        }

        [HttpPost, ActionName("SaveSchedule")]
        public JsonResult SaveSchedule(int payerGroupId, string dailyCensus, string ppdInfo)
        {
            if (ppdInfo == null || dailyCensus == null)
            {
                return new JsonResult(new { });
            }
            if (!decimal.TryParse(dailyCensus, out decimal census))
            {
                return new JsonResult(new { });
            }
            ppdInfo = ppdInfo.TrimEnd(',');
            SchdlPayerGroup payerGroup = Context.Group.Find(payerGroupId);
            foreach (string ppdGroup in ppdInfo.Split(','))
            {
                if (!decimal.TryParse(ppdGroup.Split(';')[0], out decimal ppdCount))
                {
                    return new JsonResult(new { });
                }
                string code = MapCodes(ppdGroup.Split(';')[1])[0];
                payerGroup.ScheduleLedger.Where(a => a.GeneralLedger.AccountNbr == code).FirstOrDefault().HourPPDCnt = ppdCount;

            }
            payerGroup.AvgDailyCensusCnt = census;
            Context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost, ActionName("SaveSlotEdit")]
        public JsonResult SaveSlotEdit(string slotId, string employeeId, string startShifts, string endShifts, string rooms, string hourCount)
        {

            int id;
            if (!int.TryParse(slotId, out id))
            {
                return new JsonResult(new { });
            }
            SchdlSlot slot = Context.Slots.Find(id);
            slot.EmployeeId = null;
            slot.SchdlSlotAltId = null;
            int communityID = slot.Ledger.PayerGroup.PayPeriod.CommunityId;
            int employee;
            if (!int.TryParse(employeeId, out employee))
            {
                return new JsonResult(new { });
            }
            if (employee > 0)
            {
                slot.EmployeeId = employee;
            }
            else
            {
                slot.SchdlSlotAltId = employee * -1;
            }
            Dictionary<DateTime, string> startShiftValues = GetValuesFromString(startShifts);
            Dictionary<DateTime, string> endShiftValues = GetValuesFromString(endShifts);
            Dictionary<DateTime, string> roomValues = GetValuesFromString(rooms);
            Dictionary<DateTime, string> hourValues = GetValuesFromString(hourCount);
            for (int i = 0; i < slot.Days.Count; i++)
            {
                SchdlSlotDay day = slot.Days[i];
                day.ShiftStartTime = null;
                day.ShiftEndTime = null;
                day.SchdlHourAltId = null;
                day.HourCnt = null;
                day.AtriumPatientGroupId = null;

                DateTime date = day.WorkDate;
                string startShift = startShiftValues[day.WorkDate];
                int shiftTime;
                if (int.TryParse(startShift, out shiftTime))
                {
                    if (shiftTime < 0)
                    {
                        day.SchdlHourAltId = shiftTime * -1;

                    }
                    else
                    {
                        int hour = (int)shiftTime / 100;
                        int minute = shiftTime - (hour * 100);
                        DateTime startTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                        day.ShiftStartTime = startTime;
                    }
                }
                string endShift = endShiftValues[day.WorkDate];
                if (int.TryParse(endShift, out shiftTime))
                {
                    int hour = (int)shiftTime / 100;
                    int minute = shiftTime - (hour * 100);
                    DateTime endTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                    day.ShiftEndTime = endTime;
                }
                else
                {
                    day.ShiftStartTime = null;
                }

                string room = roomValues[day.WorkDate];
                int AtriumPatientGroupId;
                if (int.TryParse(room, out AtriumPatientGroupId))
                {
                    day.AtriumPatientGroupId = AtriumPatientGroupId;
                }
                string count = hourValues[day.WorkDate];
                decimal shiftLength;
                if (decimal.TryParse(count, out shiftLength))
                {
                    if (shiftLength == 0.0m)
                    {
                        day.HourCnt = null;
                    }
                    else
                    {
                        day.HourCnt = shiftLength;
                    }
                }
            }
            Context.ChangeTracker.DetectChanges();
            Context.SaveChanges();
            if (Context.ChangeTracker.HasChanges())
            {
                Context.ChangeTracker.DetectChanges();
            }
            return Json(new { success = true });
        }

        [HttpPost, ActionName("RemoveSlot")]
        public JsonResult RemoveSlot(string slotId)
        {
            int id;
            if (!int.TryParse(slotId, out id))
            {
                return new JsonResult(new { });
            }

            SchdlSlot slot = Context.Slots.Find(id);
            int communityID = slot.Ledger.PayerGroup.PayPeriod.CommunityId;
            string payperiodDate = slot.Ledger.PayerGroup.PayPeriod.PayPeriodBeginDate.ToString("MM/dd/yyyy");
            Context.Slots.Remove(slot);
            Context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost, ActionName("AddSlot")]
        public ActionResult AddSlot(string shift, string generalLedgerId)
        {
            int id;
            if (!int.TryParse(generalLedgerId, out id))
            {
                return View();
            }
            int workShift;
            if (!int.TryParse(shift, out workShift))
            {
                return View();
            }
            SchdlGeneralLedger generalLedger = Context.SchedLedger.Find(id);

            int slotNumber = 1;
            if (generalLedger.Slots.Count > 0)
            {
                slotNumber = generalLedger.Slots.OrderByDescending(a => a.SlotNbr).FirstOrDefault().SlotNbr + 1;
            }

            SchdlSlot slot = new SchdlSlot
            {
                SchdlGeneralLedgerId = id,
                WorkShiftId = workShift,
                SlotNbr = slotNumber
            };
            Context.Slots.Add(slot);
            Context.SaveChanges();
            DateTime startDate = generalLedger.PayerGroup.PayPeriod.PayPeriodBeginDate;
            for (int i = 0; i < 14; i++)
            {
                SchdlSlotDay day = new SchdlSlotDay
                {
                    SchdlSlotId = slot.Id,
                    WorkDate = startDate.AddDays(i)
                };
                Context.SlotDays.Add(day);
            }
            Context.SaveChanges();
            return EditSlot(slot.Id.ToString());
        }

        [HttpPost, ActionName("RemoveSchedule")]
        public JsonResult RemoveSchedule(string payPeriodId)
        {
            int id;
            if (!int.TryParse(payPeriodId, out id))
            {
                return new JsonResult(new { });
            }

            SchdlPayPeriod payPeriod = Context.Week.Find(id);
            LogManager.LogScheduleDelete(payPeriod.CommunityId, payPeriod.PayPeriodBeginDate, UserPrincipal.Name);
            Context.Week.Remove(payPeriod);
            Context.SaveChanges();

            return Json(new { success = true });
        }


        [HttpPost]
        public ActionResult GetPayPeriodSchedule(int communityId, string payPeriod)
        {


            DateTime startDate = PayPeriodScheduleViewModel.ParseFirstDateofWeek(payPeriod);
            Community community = Context.Facilities.Find(communityId);
            SchdlPayPeriod weeklySchedule = Context.Week.Where(a => a.PayPeriodBeginDate == startDate
            && a.CommunityId == communityId).FirstOrDefault();
            //SchdlPayPeriod weeklySchedule =
            //(from week in Context.Week
            // where week.PayPeriodBeginDate == startDate
            //     && week.CommunityId == communityId
            // select week).FirstOrDefault();
            if (weeklySchedule == null)
            {
                return NotFound(new { Message = "A schedule for " + community.CommunityShortName + " week of " + payPeriod + " does not exit." });
            }

            PayPeriodScheduleViewModel viewModel = new PayPeriodScheduleViewModel()
            {
                CurrentCommunity = communityId,
                PayPeriodStart = startDate,
                PayPeriod = weeklySchedule,
                PayerGroup = weeklySchedule.PayerGroups.ToList<SchdlPayerGroup>(),
                Hours = new List<TotalHoursSummary>(),
                IsDisabled = false,
                PPDSummary = new List<PPDSummary>()
            };
            if (startDate.AddDays(14).Date < DateTime.Today)
            {
                viewModel.IsDisabled = true;
            }

            //Pay Period Group
            //Employee totals groupped by account number
            //Totals groupped by account number
            foreach (SchdlPayerGroup group in viewModel.PayerGroup)
            {
                decimal dailyCensus = group.AvgDailyCensusCnt;
                decimal census = dailyCensus * 14;


                foreach (SchdlGeneralLedger ledger in group.ScheduleLedger)
                {
                    decimal budget = ledger.HourPPDCnt;
                    decimal budgetedWeeklyHours = (census * budget) / 2;

                    decimal totalPPDHours = ledger.Slots.Where(a => a.EmployeeId.HasValue ? true : a.SchdlSlotAltId.HasValue
                        ? a.SchdlSlotAlt.PPDCalcFlg : false).Sum(b => b.Days.Sum(c => c.HourCnt.HasValue ? c.HourCnt.Value : 0));

                    decimal totalEmployeeHours = ledger.Slots.Where(a => a.EmployeeId.HasValue ? true : a.SchdlSlotAltId.HasValue
                        ? a.SchdlSlotAlt.EmployeeCalcFlg : false).Sum(b => b.Days.Sum(c => c.HourCnt.HasValue ? c.HourCnt.Value : 0));

                    decimal scheduledHours = 0;

                    if (census != 0)
                    {
                        scheduledHours = totalPPDHours / census;
                    }

                    string jobType = MapCodes(ledger.GeneralLedger.AccountNbr)[0];

                    PPDSummary ppd = new PPDSummary()
                    {
                        PPD = jobType,
                        Budgeted = budget,
                        Schedule = scheduledHours,
                        BudgetedHoursPerWeek = budgetedWeeklyHours,
                        TotalPPDHours = totalPPDHours,
                        TotalEmpHours = totalEmployeeHours
                    };
                    viewModel.PPDSummary.Add(ppd);
                    Dictionary<Employee, decimal> employeeHourCount = new Dictionary<Employee, decimal>();


                    foreach (SchdlSlot slot in ledger.Slots.Where(a => a.Employee != null))
                    {
                        if (!employeeHourCount.ContainsKey(slot.Employee))
                        {
                            employeeHourCount.Add(slot.Employee, 0);
                        }
                        foreach (SchdlSlotDay day in slot.Days.Where(a => a.HourCnt != null && a.SchdlHourAltId == null))
                        {
                            employeeHourCount[slot.Employee] += day.HourCnt.Value;
                        }
                    }

                    TotalHoursSummary summary = new TotalHoursSummary()
                    {
                        Key = MapCodes(ledger.GeneralLedger.AccountNbr)[0],
                        EmployeeTotals = employeeHourCount,
                        PayerGroupCode = group.PayerGroup.AtriumPayerGroupCode
                    };


                    viewModel.Hours.Add(summary);
                }

            }
            return PartialView("SchedulePartial", viewModel);
        }

        public JsonResult UpdateTextOpenShift(string communityName, string employeeIds, string message)
        {
            JsonResult jr = new JsonResult(new { });

            if (String.IsNullOrWhiteSpace(message))
            {
                jr.Value = new { Success = "Could not find message; null." };
            }
            if (employeeIds == null || employeeIds.Trim().Length == 0)
            {
                jr.Value = new { Success = "Could not find Employees to notify; null." };
            }
            // Get Nurses ContactInformation
            List<string> employees = employeeIds.Split('~').ToList<string>();
            IList<int> nurses = employees.Select(e =>
                int.Parse(e.Substring(0,
                e.IndexOf(',') >= 0 ? e.IndexOf(',') : e.Length
                ))).ToList<int>();
            IList<string> contacts = FindNurseContactInformation(nurses);

            try
            {
                MailHelper.SendEmailsBcc(null, contacts,
                        String.Format("Open Shift Notification - {0}", communityName),
                        message ?? "Open shift available. Please contact Supervisor.",
                        false
                    );
                jr.Value = new { Success = "successful." };
            }
            catch (SystemException se)
            {
                jr.Value = new { Failure = " failed." + Environment.NewLine + WrapException(se) };
            }
            return jr;
        }


        private IList<string> FindNurseContactInformation(IList<int> nurses)
        {
            if (nurses == null || nurses.Count == 0) return null;

            List<EmployeeContact> items = (from contracts in Context.EmployeeContacts
                                           join employees in Context.Employees
                                           on contracts.Id equals employees.EmployeeId
                                           where employees.JobClasses.Any(a => a.JobClass.JobDescription.Contains("nurs") 
                                           && a.StartDate <= DateTime.Now && a.StopDate >= DateTime.Now)
                                           && !employees.JobClasses.Any(a => a.JobClass.JobDescription.Contains("non"))
                                           && employees.EmployeeStatus == "Active"
                                           && contracts.Contactinformation.Length > 0
                                           select contracts).ToList<EmployeeContact>();
            IList<string> contacts = items.Where(ec => nurses.Contains(ec.Id)).Select(e => e.Contactinformation).ToList<string>();
            return contacts;
        }
        public PartialViewResult EditTextOpenShiftEmployeesFiltered(int? communityId, string jobKeys)
        {
            int commId = communityId ?? CurrentFacility[AppCode];
            string[] keys = jobKeys.Split(',');
            IDictionary<int, string> jobTypeMappings = JobTypes
                .Where(jt => keys.Contains(jt.Key.ToString())).ToDictionary(t => t.Key, t => t.Value);

            TextOpenShiftEmployees tose = new TextOpenShiftEmployees();
            List<SelectListItem> nurses = GetNurseWithContractInfo(commId, jobTypeMappings);
            tose.textOpenShiftEmployees = nurses.Select(nurse => new TextOpenShiftEmployee
            {
                EmployeeId = nurse.Value,
                EmployeeName = nurse.Text,
                Selected = true
            });
            return PartialView(tose);
        }

        public PartialViewResult EditTextOpenShift(int? communityId, string payPeriod)
        {
            IDictionary<int, string> jobTypeMappings = JobTypes;
            int commId = communityId ?? CurrentFacility[AppCode];
            List<SelectListItem> nurses = GetNurseWithContractInfo(commId, jobTypeMappings);

            TextOpenShift tos = new TextOpenShift
            {
                CommunityId = communityId ?? CurrentFacility[AppCode]
            };
            tos.CommunityName = Context.Facilities
                                    .Where(c => c.CommunityId == tos.CommunityId)
                                    .Select(c => c.CommunityShortName).FirstOrDefault() ?? "";
            tos.PayPeriod = TextOpenShift.ParseFirstDateofWeek(payPeriod ?? "").ToString("yyyy MMMM dd");
            tos.jobType = jobTypeMappings.Select(jtm => new TextOpenShiftJobTypes
            {
                JobTypeId = jtm.Key,
                JobTypeName = jtm.Value,
                Selected = true
            });
            tos.Employees.textOpenShiftEmployees = nurses.Select(nurse => new TextOpenShiftEmployee
            {
                EmployeeId = nurse.Value,
                EmployeeName = nurse.Text,
                Selected = true
            });

            return PartialView(tos);
        }

        // NOTE: Paging list should always have ACTIVE Employees only.
        private List<SelectListItem> GetNurseWithContractInfo(int? communityId, IDictionary<int, string> jobTypes)
        {
            if (communityId == null) return null;
            if (jobTypes == null || jobTypes.Count == 0) return null;

            var items = Context.EmployeeContacts.Include(a => a.Employee.JobClasses.Select(c => c.GLAccount)).Where(a =>
                a.Employee.JobClasses.Any(b =>
                    b.JobClass.JobDescription.Contains("nurs")
                    && b.StartDate <= DateTime.Now && b.StopDate >= DateTime.Now
                    && !b.JobClass.JobDescription.Contains("non"))
                && a.Employee.EmployeeStatus == "Active" 
                && a.Employee.CommunityId == communityId
                && a.Contactinformation.Length > 0).ToList().Select(a => a.Employee).OrderBy(a => a.LastName).ThenBy(a => a.FirstName).ToList();

            //List<Employee> items = (from employees in Context.Employees
            //                        join contracts in Context.EmployeeContacts
            //                        on employees.EmployeeId equals contracts.Id
            //                        where employees.CommunityId == communityId
            //                        && employees.JobClasses.Any(a => a.JobClass.JobDescription.Contains("nurs"))
            //                        && !employees.JobClasses.Any(a => a.JobClass.JobDescription.Contains("non"))
            //                        && employees.EmployeeStatus == "Active"
            //                        && contracts.Contactinformation.Length > 0
            //                        select employees).OrderBy(n => n.LastName).ThenBy(n => n.FirstName).ToList<Employee>();
            int maxLength = 0;
            if (items != null && items.Count() > 0)
                maxLength = items.Max(f => f.FirstName.Length) + items.Max(l => l.LastName.Length) + 1;
            var rnList = MapCodes("RN");
            var lpnList = MapCodes("LPN");
            var cnaList = MapCodes("CNA");

            List<SelectListItem> firstFiltered = new List<SelectListItem>();
            foreach(var employee in items)
            {
                var value = employee.EmployeeId + ",";
                string text = string.Empty;
                if(employee.JobClasses.Any(a => rnList.Contains(a.GLAccount.AccountNbr)))
                {
                    value += "RN";
                    text = String.Format("{0,-" + maxLength.ToString() + "} {1}",
                        String.Format("{0}, {1}", employee.LastName, employee.FirstName),
                        ("RN")
                    ).Replace(" ", "\u00A0");
                    firstFiltered.Add(new SelectListItem() { Text = text, Value = value });
                }
                else if (employee.JobClasses.Any(a => lpnList.Contains(a.GLAccount.AccountNbr)))
                {
                    value += "LPN";
                    text = String.Format("{0,-" + maxLength.ToString() + "} {1}",
                        String.Format("{0}, {1}", employee.LastName, employee.FirstName),
                        ("LPN")
                    ).Replace(" ", "\u00A0");
                    firstFiltered.Add(new SelectListItem() { Text = text, Value = value });
                }
                else if (employee.JobClasses.Any(a => cnaList.Contains(a.GLAccount.AccountNbr)))
                {
                    value += "CNA";
                    text = String.Format("{0,-" + maxLength.ToString() + "} {1}",
                        String.Format("{0}, {1}", employee.LastName, employee.FirstName),
                        ("CNA")
                    ).Replace(" ", "\u00A0");
                    firstFiltered.Add(new SelectListItem() { Text = text, Value = value });
                }
                
            }

            if (jobTypes.Count > 0 && jobTypes.Count < 3)
            {
                List<string> currentTypes = jobTypes.Select(t => t.Value).ToList<string>();
                if (currentTypes != null && currentTypes.Count > 0)
                {
                    return firstFiltered.Where(nurse =>
                        currentTypes.Contains(nurse.Value.Substring(nurse.Value.IndexOf(',') + 1).Replace("&nbsp;", "")))
                        .ToList<SelectListItem>();
                }
            }
            firstFiltered = PushOpenPosition(firstFiltered);
            //InsertOpenPositionToEmployeeSelectList(firstFiltered);
            return firstFiltered.ToList<SelectListItem>();
        }

        private List<SelectListItem> PushOpenPosition(IList<SelectListItem> employees)
        {
            IList<SelectListItem> pushedEmployees;
            if (employees == null)
            {
                pushedEmployees = new List<SelectListItem>();
            }
            else
            {
                pushedEmployees = employees;
            }

            if (!pushedEmployees.Contains(OPEN_POSITION))
            {
                OPEN_POSITION.Selected = true;
                pushedEmployees.Insert(0, OPEN_POSITION);
            }
            if (!pushedEmployees.Contains(PENDING_POSITION))
            {
                pushedEmployees.Insert(1, PENDING_POSITION);
            }
            return pushedEmployees.ToList();
        }

        private static SelectListItem OPEN_POSITION = new SelectListItem
        {
            Value = "-1",
            Text = "Open Position",
            Selected = false
        };

        private static SelectListItem PENDING_POSITION = new SelectListItem
        {
            Value = "-2",
            Text = "Pending Position",
            Selected = false
        };

        public static string WrapException(Exception ex)
        {
            return ex != null ?
                String.Format("Exception=[{1}]{0}StackTrace=[{2}]{0}InnerException=[{3}]{0}InnerStackTrace=[{4}]",
                    Environment.NewLine, ex.Message ?? "", ex.StackTrace ?? "",
                    ex.InnerException.Message ?? "", ex.InnerException.StackTrace ?? "") :
                "";
        }

        [HttpGet, ActionName("CurrentPayPeriodDates")]
        public JsonResult PayPeriodDates(int? communityId)
        {
            if (communityId == null || communityId < 1)
            {
                return new JsonResult(new { Success = false });
            }

            JsonResult json = new JsonResult(new
            {
                Success = true,
                PayPeriodBeginDates = String.Join(",", PayPeriodBeginDates(communityId).Select(dt => dt.ToString("yyyyMMdd")).ToArray())
            });

            return json;
        }

        private void LogCreateEditSchedule(int communityId, DateTime payPeriod)
        {
            EnsureCommunityIdIsAssignedToCurrentFacility(communityId);
            bool ScheduleExists = ScheduleForWeekExist(communityId, payPeriod);
            LogManager.LogScheduleEntry(communityId, payPeriod, UserPrincipal.Name, ScheduleExists);
        }

        private void EnsureCommunityIdIsAssignedToCurrentFacility(int communityId)
        {
            if (communityId > 0 && (CurrentFacility[AppCode] < 1 || CurrentFacility[AppCode] != communityId))
                CurrentFacility[AppCode] = communityId;
        }
        public IActionResult GetEmployeeJobs(int communityId, DateTime? lookupDate)
        {
            ViewBag.rnList = MapCodes("RN");
            ViewBag.lpnList = MapCodes("LPN");
            ViewBag.cnaList = MapCodes("CNA");
            DateTime date = lookupDate ?? DateTime.Now;
            var employees = Context.Employees.Include("JobClasses.JobClass").Include("JobClasses.GLAccount").Where(a => a.CommunityId == communityId
            && a.EmployeeStatus == "Active").ToList();
            return PartialView("DisplayTemplates/EmployeeJobList", employees);
        }
    }


}
