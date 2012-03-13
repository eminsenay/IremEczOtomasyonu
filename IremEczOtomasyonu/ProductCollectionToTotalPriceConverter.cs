using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace IremEczOtomasyonu
{
    class ProductCollectionToTotalPriceConverter: IValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return "5";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal sumPrice = 0;
            ItemCollection collection = value as ItemCollection;
            if (collection == null)
            {
                return sumPrice.ToString();
            }
            sumPrice = collection.Cast<ProductSale>().Sum(productSale => productSale.Price);
            return sumPrice.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
