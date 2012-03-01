using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Validates if the input field is empty.
    /// </summary>
    class NotEmptyValidationRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Lütfen bir değer giriniz.");
            }
            return new ValidationResult(true, null);
        }
    }

    internal class MoneyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Lütfen geçerli bir değer giriniz.");
            }
            string moneyValStr = value.ToString();
            if (string.IsNullOrEmpty(moneyValStr))
            {
                return new ValidationResult(false, "Lütfen geçerli bir değer giriniz.");
            }
            
            decimal moneyVal;
            if (!decimal.TryParse(moneyValStr, out moneyVal))
            {
                return new ValidationResult(false, "Lütfen geçerli bir değer giriniz.");
            }

            return new ValidationResult(true, null);
        }
    }
}
