using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class TextOpenShift
    {
        //public TextOpenShift(IDictionary<int, string> values, List<SelectListItem> employees) { }

        public TextOpenShift()
        {
            Employees = new TextOpenShiftEmployees();
        }

        public bool IsAdministrator { get; set; }

        public int CommunityId { get; set; }
        public string CommunityName { get; set; }
        public string PayPeriod { get; set; }
        public TextOpenShiftEmployees Employees { get; set; }
        public IEnumerable<TextOpenShiftJobTypes> jobType { get; set; }
        public string Message { get; set; }
        public String PayPeriodDateFormatted
        {
            get
            {
                return ParseFirstDateofWeek(PayPeriod).ToString("yyyy MMMM dd");
            }
        }
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
}
