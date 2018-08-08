using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class EmployeeContact
    {
        [Required]
        public int Id { get; set; }

        public string EmployeeContactTypeId { get; set; }
        public string Contactinformation { get; set; }
        
        [ForeignKey("Id")]
        public virtual Employee Employee { get; set; }
    }
}