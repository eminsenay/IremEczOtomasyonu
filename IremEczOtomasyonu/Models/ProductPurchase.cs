using System;
using System.Collections.Generic;

namespace IremEczOtomasyonu.Models
{
    public partial class ProductPurchase
    {
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Remarks { get; set; }
        public int NumItems
        {
            get
            {
                return _NumItems;
            }
            set
            {
                OnNumItemsChanging(value);
                _NumItems = value;
            }
        }
        private int _NumItems;
        partial void OnNumItemsChanging(int value);
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public DateTime ExDate { get; set; }

        public Product Product { get; set; }
    }
}
