using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu.BL
{
    partial class SaleItem
    {
        private ExpirationDate _expDate;
        
        public ExpirationDate ExpDate
        {
            get { return _expDate ?? new ExpirationDate {ExDate = ExDate}; }
            set { _expDate = value; }
        }
    }
}
