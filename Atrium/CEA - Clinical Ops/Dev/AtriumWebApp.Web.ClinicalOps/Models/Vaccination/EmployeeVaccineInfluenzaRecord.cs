using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class EmployeeVaccineInfluenzaRecord : EmployeeVaccineRecord
    {
        public DateTime? VaccineDate { get; set; }
    }
}