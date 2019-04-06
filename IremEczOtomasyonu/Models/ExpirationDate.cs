using System;
using System.Collections.Generic;

namespace IremEczOtomasyonu.Models
{
    public partial class ExpirationDate
    {
        public DateTime ExDate { get; set; }
        public int NumItems { get; set; }
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public Product Product { get; set; }
    }
}
