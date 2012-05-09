using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu.BL
{
    partial class SaleItem
    {
        public int PrevNumSold { get; set; }
        partial void OnNumSoldChanging(int value)
        {
            PrevNumSold = NumSold;
        }
    }
}
