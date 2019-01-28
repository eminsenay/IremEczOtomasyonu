using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace IremEczOtomasyonu.ViewModel
{
    public class AddNewProductViewModel: BindableBase, IDataErrorInfo
    {
        private string _barcode;
        private string _name;
        private string _brand;
        private decimal _currentBuyingPrice;
        private decimal _currentSellingPrice;


        //public Product Product { get; private set; }

        public String Barcode
        {
            get { return _barcode; }
            set
            {
                SetProperty(ref _barcode, value);
                //Product.Barcode = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
                //Product.Name = value;
            }
        }

        public string Brand
        {
            get { return _brand; }
            set
            {
                SetProperty(ref _brand, value);
                //Product.Brand = value;
            }
        }

        public decimal CurrentBuyingPrice
        {
            get { return _currentBuyingPrice; }
            set
            {
                SetProperty(ref _currentBuyingPrice, value);
                //Product.CurrentBuyingPrice = Decimal.Parse(value);
            }
        }

        public decimal CurrentSellingPrice
        {
            get { return _currentSellingPrice; }
            set
            {
                SetProperty(ref _currentSellingPrice, value);
                //Product.CurrentSellingPrice = value;
            }
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public AddNewProductViewModel()
        {
            //Product = new Product();
            OkCommand = new DelegateCommand(OnOk, CanOk);
            CancelCommand = new DelegateCommand(OnCancel);
        }

        private void OnOk()
        {

        }

        private bool CanOk()
        {
            return true;
        }

        private void OnCancel()
        {
            //throw new NotImplementedException();
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    // Empty checks of the non-empty string fields are done here because validationrule classes are 
                    // first fired when the user enters a value but these are fired directly after initialization.
                    case "Barcode":
                        if (string.IsNullOrEmpty(Barcode) || Barcode.Trim() == string.Empty)
                        {
                            return "Lütfen bir değer giriniz.";
                        }
                        break;
                    case "Name":
                        if (string.IsNullOrEmpty(Name) || Name.Trim() == string.Empty)
                        {
                            return "Lütfen bir isim giriniz.";
                        }
                        break;
                    case "Brand":
                        if (string.IsNullOrEmpty(Brand) || Brand.Trim() == string.Empty)
                        {
                            return "Lütfen bir marka ismi giriniz.";
                        }
                        break;
                    case "CurrentBuyingPrice":
                        //return ValidateMoney(CurrentBuyingPrice);
                        break;
                    case "CurrentSellingPrice":
                        //return ValidateMoney(CurrentSellingPrice);
                        break;
                }
                return null;
            }
        }

        public string Error
        {
            get { return null; }
        }

        private string ValidateMoney(string moneyValue)
        {
            if (string.IsNullOrEmpty(moneyValue))
            {
                return "Lütfen geçerli bir değer giriniz.";
            }

            // The value can contain the currency symbol
            RegionInfo info = new RegionInfo("tr-TR");
            int currencySymbolLocation = moneyValue.IndexOf(info.CurrencySymbol, StringComparison.Ordinal);
            if (currencySymbolLocation != -1)
            {
                moneyValue = moneyValue.Remove(currencySymbolLocation, info.CurrencySymbol.Length);
            }

            decimal moneyVal;
            if (!decimal.TryParse(moneyValue, out moneyVal) || moneyVal < 0)
            {
                return "Lütfen geçerli bir değer giriniz.";
            }

            return null;
        }
    }
}
