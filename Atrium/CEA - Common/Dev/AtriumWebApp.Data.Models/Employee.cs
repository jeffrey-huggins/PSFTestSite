using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string SrcSystemEmployeeId { get; set; }
        public string SrcSystemCompanyId { get; set; }
        public string SrcSystemName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public int CommunityId { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string EmployeeStatus { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string TerminationType { get; set; }
        //public string JobClass { get; set; }
        //public int? JobClassGeneralLedgerId { get; set; }
        [JsonIgnore]
        public virtual List<EmployeeJobClass> JobClasses { get; set; }
        
        [ForeignKey("CommunityId")]
        public virtual Community Community { get; set; }
    }
}