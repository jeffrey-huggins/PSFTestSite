using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientVaccinePneumonia : PatientVaccine
    {
        public int VaccinePneumoniaTypeId { get; set; }
        public DateTime? VaccineDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string LotNumber { get; set; }

    }
}