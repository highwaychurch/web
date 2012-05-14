using System.ComponentModel.DataAnnotations;

namespace Highway.Shared.Mvc.Validation
{
    public class BoolAttribute : ValidationAttribute
    {
        public bool Value { get; set; }

        public override bool IsValid(object value)
        {
            return value != null && value is bool && (bool)value == Value;
        } 
    }
}