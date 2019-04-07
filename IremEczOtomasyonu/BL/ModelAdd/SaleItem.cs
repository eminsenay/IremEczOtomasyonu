using System.ComponentModel.DataAnnotations.Schema;

namespace IremEczOtomasyonu.Models
{
    partial class SaleItem
    {
        [NotMapped]
        public int PrevNumSold { get; set; }
// ReSharper disable UnusedParameter.Local
        partial void OnNumSoldChanging(int value)
// ReSharper restore UnusedParameter.Local
        {
            PrevNumSold = NumSold;
        }
    }
}
