using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class MasterAtriumPatientGroup : IValidatableObject
    {
        // MasterAtriumPatientId
        [Required]
        public int Id { get; set; }
        public int CommunityId { get; set; }
        public string AtriumPatientGroupName { get; set; }
        //public DateTime InsertDate { get; set; }
        //public DateTime LastModifiedDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrWhiteSpace(AtriumPatientGroupName))
            {
                yield return new ValidationResult("The AtriumPatientGroupName is required.");
            }
        }
    }
}