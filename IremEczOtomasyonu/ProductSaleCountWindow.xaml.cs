using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IremEczOtomasyonu.BL;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for ProductSaleCountWindow.xaml
    /// </summary>
    public partial class ProductSaleCountWindow : Window
    {
        public class SaleCountItem
        {
            public string ProductName { get; set; }
            public string Brand { get; set; }
            public int TotalNumSold { get; set; }
        }

        public class SaleCountDate: IDataErrorInfo
        {
            public int LastDateCount { get; set; }
            public DateTime? IntervalStartDate { get; set; }
            public DateTime? IntervalEndDate { get; set; }

            public string this[string columnName]
            {
                get
                {
                    switch (columnName)
                    {
                        case "IntervalEndDate":
                            if (IntervalEndDate.HasValue && IntervalStartDate.HasValue && 
                                IntervalEndDate.Value < IntervalStartDate.Value)
                            {
                                return "Bitiş tarihi başlangıçtan küçük olamaz.";
                            }
                            if (IntervalEndDate.HasValue && IntervalEndDate.Value.Date > DateTime.Today)
                            {
                                return "Bitiş tarihi en fazla bugün olabilir.";
                            }
                            break;
                        case "IntervalStartDate":
                            if (IntervalEndDate.HasValue && IntervalStartDate.HasValue &&
                                IntervalEndDate.Value < IntervalStartDate.Value)
                            {
                                return "Başlangıç tarihi bitişten büyük olamaz.";
                            }
                            if (IntervalStartDate.HasValue && IntervalStartDate.Value.Date > DateTime.Today)
                            {
                                return "Başlangıç tarihi en fazla bugün olabilir.";
                            }
                            break;
                    }
                    return null;
                }
            }

            public string Error
            {
                get { return null; }
            }
        }

        public ObservableCollection<SaleCountItem> SaleCountItems { get; private set; }
        public SaleCountDate SaleCntDate { get; set; }

        public ProductSaleCountWindow()
        {
            InitializeComponent();
            enterLastDateRadioButton.IsChecked = true;
            lastDateTextBox.Focus();

            SaleCountItems = new ObservableCollection<SaleCountItem>();
            CollectionViewSource productSaleCountViewSource = (
                (CollectionViewSource)(FindResource("productSaleCountViewSource")));
            productSaleCountViewSource.Source = SaleCountItems;

            SaleCntDate = new SaleCountDate {LastDateCount = 30};
            searchCriteriaGroupBox.DataContext = SaleCntDate;

            lastDateTextBox.SelectAll();
        }

        private void EnterLastDateRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            lastDateTextBox.IsEnabled = true;
            intervalStartDatePicker.IsEnabled = false;
            intervalEndDatePicker.IsEnabled = false;
            lastDateTextBox.Focus();
            lastDateTextBox.SelectAll();
        }

        private void EnterDateIntervalRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            lastDateTextBox.IsEnabled = false;
            intervalStartDatePicker.IsEnabled = true;
            intervalEndDatePicker.IsEnabled = true;
            intervalStartDatePicker.IsDropDownOpen = true;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate, endDate;

            if (lastDateTextBox.IsEnabled)
            {
                if (Validation.GetHasError(lastDateTextBox))
                {
                    MessageBox.Show("Girdiğiniz değer hatalı. Lütfen değiştirip tekrar deneyin.", "Uyarı",
                                    MessageBoxButton.OK);
                    return;
                }
                endDate = DateTime.Today;
                TimeSpan ts = new TimeSpan(SaleCntDate.LastDateCount, 0, 0, 0);
                startDate = endDate - ts;
            }
            else
            {
                if (Validation.GetHasError(intervalStartDatePicker) || Validation.GetHasError(intervalEndDatePicker))
                {
                    MessageBox.Show("Girdiğiniz değerlerden biri hatalı. Lütfen değiştirip tekrar deneyin.", "Uyarı",
                                    MessageBoxButton.OK);
                    return;
                }
                startDate = SaleCntDate.IntervalStartDate ?? DateTime.Today;
                endDate = SaleCntDate.IntervalEndDate ?? DateTime.Today;
            }
            
            // Set endDate to one day after since linq to entity doesn't support comparing by .Date property
            endDate += new TimeSpan(24, 0, 0);

            var query = from saleItem in ObjectCtx.Context.SaleItems
                        where saleItem.ProductSale.SaleDate >= startDate && saleItem.ProductSale.SaleDate <= endDate
                        group saleItem by saleItem.Product
                        into saleItemProduct
                        orderby saleItemProduct.Sum(s => s.NumSold) descending 
                        select new SaleCountItem
                               {
                                   ProductName = saleItemProduct.Key.Name,
                                   Brand = saleItemProduct.Key.Brand,
                                   TotalNumSold = saleItemProduct.Sum(saleItem => saleItem.NumSold)
                               };

            SaleCountItems.Clear();
            foreach (SaleCountItem saleCountItem in query)
            {
                SaleCountItems.Add(saleCountItem);
            }
        }
    }
}
