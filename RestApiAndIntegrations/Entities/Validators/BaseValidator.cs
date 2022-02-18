using FluentValidation;
using System;
using System.Globalization;

namespace AD.CAAPS.Entities.Validators
{
#pragma warning disable CA1710 // Identifiers should have correct suffix - this is a *Validator* and not a Collection
    public class BaseValidator<T> : AbstractValidator<T>
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        public bool BeAValidDate(string value)
        {
            return DateTime.TryParseExact(value, "yyyy-mm-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _);
        }

        public bool BeGreaterThanZero<TValue>(TValue value)
        {
            if (Convert.ToDecimal(value) > 0)
            {
                return true;
            }
            return false;
        }
    }
}