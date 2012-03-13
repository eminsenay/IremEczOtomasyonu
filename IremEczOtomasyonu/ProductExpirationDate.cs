using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu
{
    partial class ProductExpirationDate: IDataErrorInfo
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
