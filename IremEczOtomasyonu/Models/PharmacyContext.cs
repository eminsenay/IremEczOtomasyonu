using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IremEczOtomasyonu.Models
{
    public partial class PharmacyContext : DbContext
    {
        public PharmacyContext()
        {
        }

        public PharmacyContext(DbContextOptions<PharmacyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<ExpirationDate> ExpirationDates { get; set; }
        public virtual DbSet<ProductPurchase> ProductPurchases { get; set; }
        public virtual DbSet<ProductSale> ProductSales { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<SaleItem> SaleItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlite("Data Source=test.db;");
            }
        }
    }
}
