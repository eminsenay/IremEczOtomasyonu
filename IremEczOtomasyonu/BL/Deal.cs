using IremEczOtomasyonu.Models;
using System;

namespace IremEczOtomasyonu.BL
{
    public enum DealType { Purchase, Sale }
    
    /// <summary>
    /// Collects the purchases and sales together.
    /// </summary>
    public class Deal
    {
        public DateTime TransactionDate { get; set; }
        public DealType TransactionType { get; set; }
        public decimal UnitPrice { get; set; }
        public int NumItems { get; set; }
        public string Details { get; set; }
        public Customer Buyer { get; set; }
    }
}
