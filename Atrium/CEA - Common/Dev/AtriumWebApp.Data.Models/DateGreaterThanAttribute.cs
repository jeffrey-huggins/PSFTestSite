using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "Date {0} must be greater.";

        public DateGreaterThanAttribute(string comparePropertyName)
        {
            ComparePropertyName = comparePropertyName;
            ErrorMessage = DefaultErrorMessage;
        }

        public string ComparePropertyName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var greater = (DateTime?)value;
            var property = validationContext.ObjectType.GetProperty(ComparePropertyName);
            var lesser = (DateTime?)property.GetValue(validationContext.ObjectInstance, null);
            if (greater > lesser)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }
    }
}
