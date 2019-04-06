using System;
using System.Collections.Generic;

namespace IremEczOtomasyonu.Models
{
    public partial class Customer
    {
        public Customer()
        {
            ProductSales = new HashSet<ProductSale>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string SkinType { get; set; }
        public string MaritalStatus { get; set; }
        public string Job { get; set; }
        public string DetailedInfo { get; set; }
        public byte[] Photo { get; set; }
        public Guid Id { get; set; }
        public string PhoneHome { get; set; }
        public string PhoneMobile { get; set; }

        public virtual ICollection<ProductSale> ProductSales { get; set; }
    }
}
