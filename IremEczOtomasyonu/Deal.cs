using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu
{
    public enum DealType { Purchase, Sale }
    public class Deal
    {
        public DateTime TransactionDate { get; set; }
        public DealType TransactionType { get; set; }
        public decimal TotalPrice { get; set; }
        public int NumItems { get; set; }
        public string Details { get; set; }
    }
}
