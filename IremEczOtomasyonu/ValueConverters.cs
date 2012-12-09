using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using IremEczOtomasyonu.BL;

namespace IremEczOtomasyonu
{
    internal class SaleItemToPriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0].GetType() != typeof (int) || values[1].GetType() != typeof (decimal))
            {
                return string.Empty;
            }
            int numSold = (int) values[0];
            decimal unitPrice = (decimal) values[1];
            return string.Format(new CultureInfo("tr-TR"), "{0:C}", numSold*unitPrice);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter class to obtain the total number of the items of a product list.
    /// </summary>
    internal class ProductListToItemSumConverter : IValueConverter
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

    internal class DealTypeToNiceStringConverter : IValueConverter
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

    internal class ExpirationDatesToDateTimeListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<ExpirationDate> expirationDates = value as IEnumerable<ExpirationDate>;
            return expirationDates == null
                       ? null
                       : expirationDates.Select(expirationDate => expirationDate.ExDate).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ProductSalesToSaleItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<ProductSale> productSales = value as IEnumerable<ProductSale>;
            if (productSales == null)
            {
                return null;
            }

            ObservableCollection<SaleItem> saleItems = new ObservableCollection<SaleItem>();
            foreach (SaleItem saleItem in productSales.SelectMany(productSale => productSale.SaleItems))
            {
                saleItems.Add(saleItem);
            }
            return saleItems;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class StringTrimConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : ((string) value).Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : ((string)value).Trim();
        }
    }

    internal class ProductToBrandAndNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Product p = value as Product;
            if (p == null)
            {
                return string.Empty;
            }
            return p.Brand + " - " + p.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class TextInputToVisibilityConverter : IMultiValueConverter
    {
        public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
        {
            // Always test MultiValueConverter inputs for non-null
            // (to avoid crash bugs for views in the designer)
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];

                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }


        public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
