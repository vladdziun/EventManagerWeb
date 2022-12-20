using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class DateInTheFutureAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        var futureDate = value as DateTime?;
        var memberNames = new List<string>() { context.MemberName };

        if (futureDate != null)
        {
            if (futureDate.Value.Date < DateTime.UtcNow.Date)
            {
                return new ValidationResult("This must be a date in the future", memberNames);
            }
        }

        return ValidationResult.Success;
    }
}