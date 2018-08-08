using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientVaccineInfluenza : PatientVaccine
    {

        public DateTime? VaccineDate { get; set; }
        public string LotNumber { get; set; }
    }
}