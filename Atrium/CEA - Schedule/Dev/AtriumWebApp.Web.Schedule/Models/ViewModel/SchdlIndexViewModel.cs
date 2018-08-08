using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class SchdlIndexViewModel
    {
        public SchdlIndexViewModel()
        {
        }

        public bool IsAdministrator { get; set; }

        public bool IsDisabled { get; set; }

        [DisplayName("Community")]
        public int? CurrentCommunity { get; set; }

        public IList<SelectListItem> Communities { get; set; }
        //public IList<SelectListItem> PayPeriod { get; set; }
        public DateTime PayPeriod { get; set; }

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