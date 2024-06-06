using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Ganss.Xss;

namespace StreamingPlatform.Utils.Validation
{
    public partial class NoHtmlAllowedAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string stringValue)
            {
                Regex regex = HtmlRegex();

                string sanitizedPlanName = regex.Replace(stringValue, string.Empty);

                if (sanitizedPlanName != stringValue)
                {
                    return new ValidationResult("HTML tags are not allowed in plan name.");
                }
            }

            return ValidationResult.Success;
        }

        [GeneratedRegex(@"<.*?>", RegexOptions.Compiled)]
        private static partial Regex HtmlRegex();
    }
}
