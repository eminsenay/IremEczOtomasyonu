using System;
using System.Collections.Generic;

namespace IremEczOtomasyonu.Models
{
    public partial class ProductSale
    {
        public ProductSale()
        {
            SaleItems = new HashSet<SaleItem>();
        }

        public Guid Id { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remarks { get; set; }
        public Customer Customer { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; }
    }
}
