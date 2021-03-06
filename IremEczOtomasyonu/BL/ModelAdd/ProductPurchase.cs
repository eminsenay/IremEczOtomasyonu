﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace IremEczOtomasyonu.Models
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

        [NotMapped]
        public string Error
        {
            get { return null; }
        }

        [NotMapped]
        public int PrevNumItems { get; set; }

// ReSharper disable UnusedParameter.Local
        partial void OnNumItemsChanging(int value)
// ReSharper restore UnusedParameter.Local
        {
            PrevNumItems = NumItems;
        }
    }
}
