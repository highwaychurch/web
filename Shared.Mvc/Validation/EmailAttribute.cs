using System.ComponentModel.DataAnnotations;

namespace Highway.Shared.Mvc.Validation
{
    public class EmailAttribute : RegularExpressionAttribute
    {
        public EmailAttribute()
            : base("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$")
        {
            ErrorMessage = "Not a valid email";
        }
    }
}