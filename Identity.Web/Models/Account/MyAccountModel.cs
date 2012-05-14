using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Highway.Identity.Web.Models.Account
{
    public class MyAccountModel : IValidatableObject
    {
        [Required]
        [StringLength(5)]
        [Display(Name="First name")]
        public string FirstName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FirstName == "Mike") yield return new ValidationResult("Naughty boy.", new [] {"FirstName"});
        }
    }
}