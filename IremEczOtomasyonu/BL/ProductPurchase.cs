using System;
using System.ComponentModel;

namespace IremEczOtomasyonu.BL
{
    partial class ProductPurchase: IDataErrorInfo
    {
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
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

        public int PrevNumItems { get; set; }

// ReSharper disable UnusedParameter.Local
        partial void OnNumItemsChanging(int value)
// ReSharper restore UnusedParameter.Local
        {
            PrevNumItems = NumItems;
        }
    }
}
