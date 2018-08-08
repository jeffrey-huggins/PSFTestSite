using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class VaccineTBSite
    {
        [Key]
        public int VaccineTBSiteId { get; set; }
        public string SiteName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}