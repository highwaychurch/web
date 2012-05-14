using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace Highway.Shared.Mvc.Validation
{
    public class RequiredIfValidator : DataAnnotationsModelValidator<RequiredIfAttribute>
    {
        public RequiredIfValidator(ModelMetadata metadata, ControllerContext context, RequiredIfAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var result = new ModelClientValidationRule
                {
                    ErrorMessage = ErrorMessage,
                    ValidationType = "requiredif"
                };
            var serializer = new DataContractJsonSerializer(typeof (object[]), new[]{ Attribute.TargetValue.First().GetType() });
            using(var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, Attribute.TargetValue);

                var template = ((ViewContext) (ControllerContext)).ViewData.TemplateInfo;
                var dependentName = template.TemplateDepth == 0 ? template.GetFullHtmlFieldId(Metadata.PropertyName) : template.GetFullHtmlFieldId(string.Empty);

                result.ValidationParameters.Add("dependent", dependentName.Replace(Metadata.PropertyName, Attribute.DependentProperty));
                result.ValidationParameters.Add("matches", Encoding.UTF8.GetString(stream.ToArray()));
                result.ValidationParameters.Add("alwaysvisible", Attribute.AlwaysVisible);

                yield return result;
            }
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            // get a reference to the property this validation depends upon
            var field = Metadata.ContainerType.GetProperty(Attribute.DependentProperty);

            if (field != null)
            {
                // get the value of the dependent property
                var value = field.GetValue(container, null);

                // compare the value against the target value
                if ((value == null && Attribute.TargetValue == null) ||
                    (value != null && Attribute.TargetValue.Contains(value)))
                {

                    // match => means we should try validating this field
                    if (Attribute.IsValid(Metadata.Model) == false)
                        // validation failed - return an error
                        yield return new ModelValidationResult { Message = ErrorMessage };
                }
            }
        }
    }
}