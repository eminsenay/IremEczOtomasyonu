using System;
using System.Collections.Generic;

namespace IremEczOtomasyonu.Models
{
    public partial class ProductSale
    {
        private Customer _customer;
        public ProductSale()
        {
            SaleItems = new HashSet<SaleItem>();
        }

        public Guid Id { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remarks { get; set; }
        public Customer Customer {
            get
            {
                return _customer;
            }
            set
            {
                if (value != _customer)
                {
                    // It has to be explicitly triggered. Otherwise change of the association ((de)assigning a customer
                    // to a product sale) is not being notified.
                    _customer = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ICollection<SaleItem> SaleItems { get; set; }
    }
}
