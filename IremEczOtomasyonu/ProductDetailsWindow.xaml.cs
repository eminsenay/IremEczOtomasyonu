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
            if (Validation.GetHasError(barcodeTextBox) || Validation.GetHasError(nameTextBox) ||
                Validation.GetHasError(brandTextBox) || Validation.GetHasError(currentBuyingPriceTextBox) ||
                Validation.GetHasError(currentSellingPriceTextBox))
            {
                MessageBox.Show("Girdiğiniz bazı bilgiler eksik ya da hatalı. \n Lütfen düzeltip tekrar deneyin.",
                                "Ürün değiştirme uyarısı", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string detailedValidationMessage = _product.Validate();
            if (!string.IsNullOrEmpty(detailedValidationMessage))
            {
                MessageBox.Show(detailedValidationMessage, "Ürün değiştirme uyarısı", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

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
