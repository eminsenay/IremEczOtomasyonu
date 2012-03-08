using System;
using System.Collections.Generic;
using System.Linq;

namespace IremEczOtomasyonu
{
    public class SaleItem
    {
        public Product Product { get; set; }
        //private string _barcode;
        private decimal _price;

        //public string Barcode
        //{
        //    get { return _barcode; }
        //    set
        //    {
        //        _barcode = value;
        //        //using (Model1Container context = new Model1Container())
        //        //{
        //        //    _product = context.Products.First(p => p.Barcode == _barcode);
        //        //}
        //    }
        //}
            
        //public string BrandName 
        //{ 
        //    get
        //    {
        //        if (Product == null)
        //        {
        //            return String.Empty;
        //        }
        //        return Product.Brand ?? String.Empty;
        //    }
        //}
            
        //public string ProductName
        //{
        //    get
        //    {
        //        if (Product == null)
        //        {
        //            return String.Empty;
        //        }
        //        return Product.Name ?? String.Empty;
        //    }
        //}

        public int NumSold { get; set; }

        public decimal Price
        {
            get
            {
                if (_price != 0)
                {
                    return _price;
                }
                if (Product == null)
                {
                    return 0;
                }
                return Product.CurrentSellingPrice == null ? 0 : (decimal)Product.CurrentSellingPrice*NumSold;
            }
            set { _price = value; }
        }

        //public List<DateTime> ExpirationDates
        //{
        //    get
        //    {
        //        if (Product == null)
        //        {
        //            return null;
        //        }
        //        var exDate = from e in Product.ProductExpirationDates select e.ExpirationDate;
        //        return exDate.ToList();
        //    }
        //}
    }
}