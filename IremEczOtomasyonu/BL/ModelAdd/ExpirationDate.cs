using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace IremEczOtomasyonu.Models
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

        [NotMapped]
        public string Error
        {
            get { return null; }
        }
    }
}
