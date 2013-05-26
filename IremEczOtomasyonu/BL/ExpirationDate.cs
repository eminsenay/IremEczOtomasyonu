using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu.BL
{
    partial class ExpirationDate: IDataErrorInfo
    {
        public ExpirationDate()
        {
            ExDate = DateTime.Today;
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "ExDate":
                        if (ExDate.Date < DateTime.Today)
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
