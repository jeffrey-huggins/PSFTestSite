using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AtriumWebApp.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MatchesCalendarQuarterAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessageFormat = "Date {0} must be in the same calendar quarter as {1}.";

        public MatchesCalendarQuarterAttribute(string compareDatePropertyName)
            : this(compareDatePropertyName, compareDatePropertyName)
        { }

        public MatchesCalendarQuarterAttribute(string compareDatePropertyName, string compareDatePropertyDisplayName)
            : base()
        {
            CompareDatePropertyName = compareDatePropertyName;
            CompareDatePropertyDisplayName = compareDatePropertyDisplayName;
        }

        public string CompareDatePropertyName { get; set; }
        public string CompareDatePropertyDisplayName { get; set; }

        public override string FormatErrorMessage(string name)
        {
            var comparePropertyName = CompareDatePropertyDisplayName ?? CompareDatePropertyName;
            return string.Format(DefaultErrorMessageFormat, name, comparePropertyName);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            var comparePropertyInfo = validationContext.ObjectType.GetProperty(CompareDatePropertyName);
            var comparePropertyValue = Convert.ToDateTime(comparePropertyInfo.GetValue(validationContext.ObjectInstance, null));
            var minimumValue = comparePropertyValue.CalculateCalendarQuarterStart();
            var maximumValue = comparePropertyValue.CalculateCalendarQuarterEnd();
            var convertedValue = Convert.ToDateTime(value);
            if (convertedValue >= minimumValue && convertedValue <= maximumValue)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public override bool IsValid(object value)
        {
            throw new NotSupportedException("Overload that takes a ValidationContext must be used.");
        }
    }
}