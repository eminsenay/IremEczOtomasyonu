using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IremEczOtomasyonu
{
    public class SaleItem: INotifyPropertyChanged
    {
        public Product Product { get; set; }
        
        private decimal _price;
        private int _numSold;

        public int NumSold
        {
            get { return _numSold; } 
            set 
            { 
                _numSold = value;
                OnPropertyChanged(new PropertyChangedEventArgs("NumSold"));
                OnPropertyChanged(new PropertyChangedEventArgs("Price"));
            }
        }

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
            set
            {
                _price = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Price")); 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        } 
    }
}