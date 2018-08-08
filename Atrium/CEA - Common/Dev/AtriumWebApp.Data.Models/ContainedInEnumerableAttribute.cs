using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ContainedInEnumerableAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "{0} is not a recognized valid value.";

        public ContainedInEnumerableAttribute(string enumerablePropertyName)
            : this(enumerablePropertyName, false)
        { }

        public ContainedInEnumerableAttribute(string enumerablePropertyName, bool isEnumerablePropertyStatic)
        {
            EnumerablePropertyName = enumerablePropertyName;
            IsEnumerablePropertyStatic = isEnumerablePropertyStatic;
            ErrorMessage = DefaultErrorMessage;
        }

        public string EnumerablePropertyName { get; set; }
        public bool IsEnumerablePropertyStatic { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property;
            IEnumerable enumerable;
            if (IsEnumerablePropertyStatic)
            {
                property = validationContext.ObjectType.GetProperty(EnumerablePropertyName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                enumerable = (IEnumerable)property.GetValue(validationContext.ObjectInstance, null);
            }
            else
            {
                property = validationContext.ObjectType.GetProperty(EnumerablePropertyName);
                enumerable = (IEnumerable)property.GetValue(null, null);
            }
            if (enumerable.Cast<object>().Contains(value))
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
