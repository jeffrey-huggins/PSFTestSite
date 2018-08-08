using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "{0} is required.";

        public RequiredIfAttribute(string dependentPropertyName)
            : this(dependentPropertyName, true)
        { }

        public RequiredIfAttribute(string dependentPropertyName, object desiredValue)
        {
            DependentPropertyName = dependentPropertyName;
            DesiredValue = desiredValue;
            ErrorMessage = DefaultErrorMessage;
        }

        public string DependentPropertyName { get; set; }
        public object DesiredValue { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dependentPropertyInfo = validationContext.ObjectType.GetProperty(DependentPropertyName);
            var dependentPropertyValue = dependentPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (object.Equals(dependentPropertyValue, DesiredValue))
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }
    }
}