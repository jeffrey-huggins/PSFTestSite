using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredElementsAttribute : ValidationAttribute
    {
        public RequiredElementsAttribute()
            : base("At least one {0} must be added.")
        { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IEnumerable list = value as IEnumerable;
            if (list != null && list.GetEnumerator().MoveNext())
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
        }
    }
}