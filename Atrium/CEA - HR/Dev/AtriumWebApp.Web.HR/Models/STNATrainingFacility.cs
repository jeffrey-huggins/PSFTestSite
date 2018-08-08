using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AtriumWebApp.Web.HR.Models
{
    public class STNATrainingFacility
    {
        public int STNATrainingFacilityId { get; set; }
        [DisplayName("Training Facility Name")]
        [Required]
        [MaxLength(30)]
        public string TrainingFacilityName { get; set; }
        [MaxLength(64)]
        [DisplayName("Address 1")]
        public string Address1 { get; set; }
        [DisplayName("Address 2")]
        [MaxLength(64)]
        public string Address2 { get; set; }
        [MaxLength(32)]
        public string City { get; set; }
        [DisplayName("State")]
        [MaxLength(2)]
        public string StateCd { get; set; }
        [DisplayName("Zip")]
        [MaxLength(20)]
        public string ZipCode { get; set; }
        [DisplayName("Contact's Name")]
        [MaxLength(128)]
        public string ContactName { get; set; }
        [DisplayName("Contact's Phone Number")]
        [MaxLength(16)]
        [Phone]
        public string ContactPhone { get; set; }
        [DisplayName("Contact's E-Mail Address")]
        [MaxLength(64)]
        [EmailAddress]
        public string ContactEmail { get; set; }
        //public DateTime InsertedDate { get; set; }
        //public DateTime? LastModifiedDate { get; set; }

        public ICollection<Community> Communities { get; set; }
    }
}