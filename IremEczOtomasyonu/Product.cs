using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu
{
    partial class Product: IDataErrorInfo
    {
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Barcode":
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
                        // don't need to check it since it is checked by moneyvalidationrule
                        break;
                    case "CurrentSellingPrice":
                        // don't need to check it since it is checked by moneyvalidationrule
                        break;
                }
                return null;
            }
        }

        public string Error
        {
            get { return null; }
        }
    }
}
