using System;
using System.Collections.Generic;

namespace IremEczOtomasyonu.Models
{
    public partial class SaleItem
    {
        public int NumSold
        {
            get
            {
                return _NumSold;
            }
            set
            {
                OnNumSoldChanging(value);
                _NumSold = value;
            }
        }
        private int _NumSold;
        partial void OnNumSoldChanging(int value);

        public Guid Id { get; set; }
        public DateTime ExDate { get; set; }
        public decimal UnitPrice { get; set; }
        public Product Product { get; set; }
        public ProductSale ProductSale { get; set; }
    }
}
