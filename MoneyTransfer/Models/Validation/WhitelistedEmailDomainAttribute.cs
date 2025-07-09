using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.Models.Validation;

public class WhitelistedEmailDomainAttribute(string[] allowedDomains) : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var email = value as string;
        if (string.IsNullOrEmpty(email))
            return ValidationResult.Success;

        var atIndex = email.LastIndexOf('@');
        if (atIndex < 0 || atIndex == email.Length - 1)
            return new ValidationResult("Invalid email format.");

        var domain = email.Substring(atIndex + 1);
        if (!allowedDomains.Contains(domain, StringComparer.OrdinalIgnoreCase))
            return new ValidationResult($"Email domain '{domain}' is not allowed.");

        return ValidationResult.Success;
    }
}