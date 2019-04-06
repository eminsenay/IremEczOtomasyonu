using IremEczOtomasyonu.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace IremEczOtomasyonu.BL
{
    /// <summary>
    /// Validates if the input field is empty.
    /// </summary>
    class NotEmptyValidationRule : ValidationRule
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

    /// <summary>
    /// Validates if the given value can be used as a money value.
    /// </summary>
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

            // The value can contain the currency symbol
            RegionInfo info = new RegionInfo("tr-TR");
            int currencySymbolLocation = moneyValStr.IndexOf(info.CurrencySymbol, StringComparison.Ordinal);
            if (currencySymbolLocation != -1)
            {
                moneyValStr = moneyValStr.Remove(currencySymbolLocation, info.CurrencySymbol.Length);
            }
            
            decimal moneyVal;
            if (!decimal.TryParse(moneyValStr, out moneyVal) || moneyVal < 0)
            {
                return new ValidationResult(false, "Lütfen geçerli bir değer giriniz.");
            }

            return new ValidationResult(true, null);
        }
    }

    /// <summary>
    /// Validates if the entered barcode is unique among the product database.
    /// </summary>
    internal class UniqueBarcodeRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Lütfen geçerli bir değer giriniz.");
            }
            string barcode = value.ToString();
            if (string.IsNullOrEmpty(barcode))
            {
                return new ValidationResult(false, "Lütfen geçerli bir değer giriniz.");
            }
            using (PharmacyContext dbContext = new PharmacyContext())
            {
                if (dbContext.Products.Any(p => p.Barcode == barcode))
                {
                    return new ValidationResult(false, "Bu barkoda sahip bir ürün sistemde kayıtlı.");
                }
            }
            return new ValidationResult(true, null);
        }
    }

    /// <summary>
    /// Validates if the entered barcode is found in the product database.
    /// </summary>
    internal class FoundBarcodeRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Lütfen geçerli bir değer giriniz.");
            }
            string barcode = value.ToString();
            if (string.IsNullOrEmpty(barcode))
            {
                return new ValidationResult(false, "Lütfen geçerli bir değer giriniz.");
            }
            using (PharmacyContext dbContext = new PharmacyContext())
            {
                if (dbContext.Products.Any(p => p.Barcode == barcode))
                {
                    return new ValidationResult(true, null);
                }
            }
            return new ValidationResult(false, "Bu barkoda sahip bir ürün sistemde kayıtlı değil.");
        }
    }

    /// <summary>
    /// Validates if the given value is integer within the given limits.
    /// </summary>
    internal class IntegerValidationRule : ValidationRule
    {
        private int _min = int.MinValue;
        private int _max = int.MaxValue;

        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Lütfen geçerli bir değer giriniz.");
            }

            int num;
            if (!int.TryParse(value.ToString(), out num))
            {
                return new ValidationResult(false, "Lütfen geçerli bir değer giriniz.");
            }

            if (num < Min)
            {
                return new ValidationResult(false, string.Format("Bu değer en küçük {0} olabilir.", Min));
            }
            if (num > Max)
            {
                return new ValidationResult(false, string.Format("Bu değer en büyük {0} olabilir.", Max));
            }

            return new ValidationResult(true, null);
        }
    }
}
