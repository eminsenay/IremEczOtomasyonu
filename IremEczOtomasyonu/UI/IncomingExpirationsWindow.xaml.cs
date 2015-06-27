using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using IremEczOtomasyonu.BL;

namespace IremEczOtomasyonu.UI
{
    /// <summary>
    /// Interaction logic for IncomingExpirationsWindow.xaml
    /// </summary>
    public partial class IncomingExpirationsWindow : Window
    {
        private ObservableCollection<ExpirationDate> IncomingExpirationDates { get; set; }

        public IncomingExpirationsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource expirationDatesViewSource = ((CollectionViewSource)
                (FindResource("ExpirationDatesViewSource")));
            DateTime oneMonthLater = DateTime.Today + new TimeSpan(365, 0, 0, 0);
            IncomingExpirationDates = new ObservableCollection<ExpirationDate>(ObjectCtx.Context.ExpirationDates.Where(
                x => x.NumItems > 0 && oneMonthLater >= x.ExDate).OrderBy(x => x.ExDate));
            expirationDatesViewSource.Source = IncomingExpirationDates;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void DatagridDetailsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExpirationDate expirationDate = expirationDatesDataGrid.SelectedItem as ExpirationDate;
            if (expirationDate == null)
            {
                Debug.Fail("Selected Expiration date cannot be retrieved.");
                return;
            }

            ProductDetailsWindow productDetailsWindow = new ProductDetailsWindow(expirationDate.Product)
                                                            {Owner = Parent as Window};

            if (productDetailsWindow.ShowDialog() == true)
            {
                // Get the incoming expiration dates agai since the user may have modified them from 
                // the product details window.
                DateTime oneMonthLater = DateTime.Today + new TimeSpan(365, 0, 0, 0);
                IncomingExpirationDates.Clear();
                var coll = ObjectCtx.Context.ExpirationDates.Where(
                    x => x.NumItems > 0 && oneMonthLater >= x.ExDate).OrderBy(x => x.ExDate);
                foreach (ExpirationDate expDate in coll)
                {
                    IncomingExpirationDates.Add(expDate);
                }
            }
        }
    }
}
