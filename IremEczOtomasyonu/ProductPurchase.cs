using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu
{
    partial class ProductPurchase: IDataErrorInfo
    {
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "ExpirationDate":
                        if (ExpirationDate.Date < DateTime.Today)
                        {
                            return "Lütfen geçerli bir son kullanma tarihi giriniz.";
                        }
                        break;
                    case "PurchaseDate":
                        if (PurchaseDate.Date > DateTime.Today)
                        {
                            return "Alış tarihi bugün ya da daha önceki bir tarih olabilir.";
                        }
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
