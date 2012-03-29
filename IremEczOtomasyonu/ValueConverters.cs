using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace IremEczOtomasyonu
{
    class SaleItemToPriceConverter: IMultiValueConverter 
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0].GetType() != typeof(int) || values[1].GetType() != typeof(decimal))
            {
                return string.Empty;
            }
            int numSold = (int) values[0];
            decimal unitPrice = (decimal) values[1];
            return (numSold*unitPrice).ToString(CultureInfo.CurrentCulture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ProductListToItemSumConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<object> productList = value as IEnumerable<object>;
            if (productList == null)
            {
                return string.Empty;
            }
            int sum = 0;
            foreach (object product in productList)
            {
                Product nextProduct = product as Product;
                Debug.Assert(nextProduct != null, "Converter value parameter doesn't contain products.");
// ReSharper disable PossibleNullReferenceException
                sum += nextProduct.NumItems;
// ReSharper restore PossibleNullReferenceException
            }
            return sum.ToString(CultureInfo.CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class SaleItemListToHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ReadOnlyObservableCollection<object> saleItems = value as ReadOnlyObservableCollection<object>;
            if (saleItems == null)
            {
                return string.Empty;
            }
            SaleItem firstItem = saleItems[0] as SaleItem;
            if (firstItem == null)
            {
                return string.Empty;
            }
            ProductSale productSale = firstItem.ProductSale;
            StringBuilder ret = new StringBuilder();
            ret.AppendFormat("{0:d} {0:HH:mm}", productSale.SaleDate); 
            ret.Append(", Toplam Fiyat: " + productSale.TotalPrice);
            if (!string.IsNullOrEmpty(productSale.Remarks))
            {
                ret.Append(", Bilgi: " + productSale.Remarks);
            }
            return ret.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class DealTypeToNiceStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DealType dealType = (DealType) value;
            switch (dealType)
            {
                case DealType.Purchase:
                    return "Alış";
                case DealType.Sale:
                    return "Satış";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
