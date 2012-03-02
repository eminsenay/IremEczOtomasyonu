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
            using (Model1Container dbContext = new Model1Container())
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
            using (Model1Container dbContext = new Model1Container())
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
                string suffix = GetNumSuffixForTurkish(Min);
                return new ValidationResult(false, string.Format("Lütfen {0}'{1} büyük bir değer giriniz.", 
                    Min, suffix));
            }
            if (num > Max)
            {
                string suffix = GetNumSuffixForTurkish(Min);
                return new ValidationResult(false, string.Format("Lütfen {0}'{1} küçük bir değer giriniz.",
                    Max, suffix));
            }

            return new ValidationResult(true, null);
        }

        private string GetNumSuffixForTurkish(int number)
        {
            if (number == 0 || number%1000000 == 0) // milyon, milyar, trilyon, katrilyon...
            {
                return "dan";
            }
            int lastDigit = number%10;
            string suffix = string.Empty;
            switch (lastDigit)
            {
                case 1:
                case 2:
                case 7:
                case 8:
                    suffix = "den";
                    break;
                case 3:
                case 4:
                case 5:
                    suffix = "ten";
                    break;
                case 6:
                case 9:
                    suffix = "dan";
                    break;
                case 0:
                    int tens = Min%100;
                    switch (tens)
                    {
                        case 0: // yüz, bin
                        case 20:
                        case 50:
                        case 70:
                        case 80:
                            suffix = "den";
                            break;
                        case 10:
                        case 30:
                        case 90:
                            suffix = "dan";
                            break;
                        case 40:
                        case 60:
                            suffix = "tan";
                            break;
                    }
                    break;
            }
            return suffix;
        }
    }
}
