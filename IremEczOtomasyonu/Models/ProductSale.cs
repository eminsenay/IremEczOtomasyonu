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

        public DateTime SaleDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remarks { get; set; }
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; }
    }
}
