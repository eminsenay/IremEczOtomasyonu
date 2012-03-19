using System;
using System.Collections.Generic;
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

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for ProductDetailsWindow.xaml
    /// </summary>
    public partial class ProductDetailsWindow : Window
    {
        private readonly Model1Container _dbContext;
        private readonly Product _product;

        public ProductDetailsWindow(Product product, Model1Container dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _product = product;

            productDetailsGrid.DataContext = _product;

            CollectionViewSource expirationDatesViewSource = ((CollectionViewSource)(FindResource("expirationDatesViewSource")));
            expirationDatesViewSource.Source = _product.ExpirationDates;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Validation
            _dbContext.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void DatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            Utilities.DatePickerSelectDecade(sender as DatePicker);
        }
    }
}
