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
// ReSharper disable UnusedParameter.Local
        partial void OnNumSoldChanging(int value)
// ReSharper restore UnusedParameter.Local
        {
            PrevNumSold = NumSold;
        }
    }
}
