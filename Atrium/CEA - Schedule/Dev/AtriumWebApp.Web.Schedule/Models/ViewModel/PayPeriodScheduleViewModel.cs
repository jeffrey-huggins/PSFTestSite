using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Schedule.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class PayPeriodScheduleViewModel
    {
        public SchdlPayPeriod PayPeriod { get; set; }
        public List<SchdlPayerGroup> PayerGroup { get; set; }
        public List<TotalHoursSummary> Hours { get; set; }

        public IEnumerable<SchdlSlot> GetShiftSlots(string type, int shiftId, SchdlPayerGroup payerGroup)
        {
            List<SchdlSlot> shiftSlots = payerGroup.ScheduleLedger.Where(a => a.GeneralLedger.AccountNbr == ScheduleController.MapCodes(type)[0]).FirstOrDefault().Slots.Where(a => a.WorkShiftId == shiftId).ToList<SchdlSlot>();
            return shiftSlots.OrderBy(a => a.SlotNbr);
        }
        public SchdlGeneralLedger GetGeneralLedger(string type, int shiftId, SchdlPayerGroup payerGroup)
        {
            return payerGroup.ScheduleLedger.Where(a => a.GeneralLedger.AccountNbr == ScheduleController.MapCodes(type)[0]).FirstOrDefault();
        }
        //public bool IsAdministrator { get; set; }
        public bool CanDelete { get; set; }
        public bool CanManageTemplates { get; set; }
        public bool IsDisabled { get; set; }

        [DisplayName("Community")]
        public int? CurrentCommunity { get; set; }

        public IList<SelectListItem> Communities { get; set; }
        //public IList<SelectListItem> PayPeriod { get; set; }
        public DateTime PayPeriodStart { get; set; }

        public Dictionary<DateTime, decimal> SummaryHours(string types, int? shiftId, SchdlPayerGroup payerGroup)
        {
            Dictionary<DateTime, decimal> totals = new Dictionary<DateTime, decimal>();
            if (shiftId != null)
            {
                
                foreach (string type in types.Split(','))
                {
                    List<SchdlSlot> slots = payerGroup.ScheduleLedger.Where(a => a.GeneralLedger.AccountNbr == ScheduleController.MapCodes(type)[0]).FirstOrDefault().Slots.Where(a => a.WorkShiftId == shiftId).ToList();
                    if (slots.Count != 0)
                    {
                        foreach (SchdlSlotDay day in slots[0].Days)
                        {
                            if (!totals.ContainsKey(day.WorkDate))
                            {
                                totals[day.WorkDate] = 0;
                            }
                            totals[day.WorkDate] += slots.Where(a => a.SchdlSlotAltId.HasValue ? a.SchdlSlotAlt.SchdlCalcFlg : true).Sum(a => a.Days.Where(b => b.WorkDate == day.WorkDate && b.HourCnt.HasValue).Sum(c => c.HourCnt)).Value;
                        }
                    }
                }
            }
            else
            {
                foreach (string type in types.Split(','))
                {
                    List<SchdlSlot> slots = payerGroup.ScheduleLedger.Where(a => a.GeneralLedger.AccountNbr == ScheduleController.MapCodes(type)[0]).FirstOrDefault().Slots.ToList();
                    if (slots.Count != 0)
                    {
                        foreach (SchdlSlotDay day in slots[0].Days)
                        {
                            if (!totals.ContainsKey(day.WorkDate))
                            {
                                totals[day.WorkDate] = 0;
                            }
                            totals[day.WorkDate] += slots.Where(a => a.SchdlSlotAltId.HasValue ? a.SchdlSlotAlt.SchdlCalcFlg : true).Sum(a => a.Days.Where(b => b.WorkDate == day.WorkDate && b.HourCnt.HasValue).Sum(c => c.HourCnt)).Value;
                        }
                    }
                }
            }
            return totals;
        }

        public IList<PPDSummary> PPDSummary { get; set; }

        /// <summary>
        /// Returns on Error of Parse DateTime.MinValue -- validate on MinValue.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ParseFirstDateofWeek(string date)
        {
            if (String.IsNullOrWhiteSpace(date))
                return DateTime.MinValue;
            System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo();
            dtfi.ShortDatePattern = "yyyyMMdd";
            DateTime dt = DateTime.MinValue;
            try
            {
                dt = DateTime.ParseExact(date, "d", dtfi);
            }
            catch
            {
                try
                {
                    dtfi = new System.Globalization.DateTimeFormatInfo();
                    dtfi.ShortDatePattern = "MM/dd/yyyy";
                    dt = DateTime.ParseExact(date, "d", dtfi);
                }
                catch
                {
                    throw new System.FormatException(String.Format("Date Time format is unknown: {0}", date));
                }
            }
            return dt;
        }
    }

    public class PPDSummary
    {
        public string PPD { get; set; }
        public decimal Budgeted { get; set; }
        public decimal Schedule { get; set; }
        public decimal BudgetedHoursPerWeek { get; set; }
        public decimal TotalPPDHours { get; set; }
        public decimal TotalEmpHours { get; set; }
    }

    public class TotalHoursSummary
    {
        public string Key { get; set; }
        public string PayerGroupCode { get; set; }
        public Dictionary<Employee, decimal> EmployeeTotals { get; set; }
    }


}