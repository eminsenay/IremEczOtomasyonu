using System;
using System.Collections.Generic;
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
}
