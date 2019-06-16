using System;
using System.Collections.Generic;

namespace IremEczOtomasyonu.Models
{
    public partial class Product
    {
        public Product()
        {
            ExpirationDates = new HashSet<ExpirationDate>();
            ProductPurchases = new HashSet<ProductPurchase>();
        }

        public Guid Id { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public int NumItems { get; set; }
        public decimal? CurrentBuyingPrice { get; set; }
        public decimal? CurrentSellingPrice { get; set; }
        public ICollection<ExpirationDate> ExpirationDates { get; set; }
        public ICollection<ProductPurchase> ProductPurchases { get; set; }
    }
}
