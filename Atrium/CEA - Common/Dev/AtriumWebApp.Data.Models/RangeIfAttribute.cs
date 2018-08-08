using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class RangeIfAttribute : RangeAttribute
    {
        public RangeIfAttribute(double minimum, double maximum, string dependentPropertyName)
            : this(minimum, maximum, dependentPropertyName, true)
        { }

        public RangeIfAttribute(double minimum, double maximum, string dependentPropertyName, object desiredValue)
            : base(minimum, maximum)
        {
            DependentPropertyName = dependentPropertyName;
            DesiredValue = desiredValue;
        }

        public RangeIfAttribute(int minimum, int maximum, string dependentPropertyName)
            : this(minimum, maximum, dependentPropertyName, true)
        { }

        public RangeIfAttribute(int minimum, int maximum, string dependentPropertyName, object desiredValue)
            : base(minimum, maximum)
        {
            DependentPropertyName = dependentPropertyName;
            DesiredValue = desiredValue;
        }

        public RangeIfAttribute(Type type, string minimum, string maximum, string dependentPropertyName)
            : this(type, minimum, maximum, dependentPropertyName, true)
        { }

        public RangeIfAttribute(Type type, string minimum, string maximum, string dependentPropertyName, object desiredValue)
            : base(type, minimum, maximum)
        {
            DependentPropertyName = dependentPropertyName;
            DesiredValue = desiredValue;
        }

        private object _typeId = new object();
        public override object TypeId
        {
            get { return this._typeId; }
        }
        public string DependentPropertyName { get; set; }
        public object DesiredValue { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dependentPropertyInfo = validationContext.ObjectType.GetProperty(DependentPropertyName);
            var dependentPropertyValue = dependentPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (object.Equals(dependentPropertyValue, DesiredValue))
            {
                if (!base.IsValid(value))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
}