using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class VaccineType
    {
        [Key]
        public int VaccineTypeId { get; set; }
        public string VaccineTypeName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}