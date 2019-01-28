using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IremEczOtomasyonu.View
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddNewProductView : Window
    {
        public AddNewProductView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(barcodeTextBox) || Validation.GetHasError(nameTextBox) ||
                Validation.GetHasError(brandTextBox) || Validation.GetHasError(currentBuyingPriceTextBox) || 
                Validation.GetHasError(currentSellingPriceTextBox))
            {
                MessageBox.Show("Girdiğiniz bazı bilgiler eksik ya da hatalı. \n Lütfen düzeltip tekrar deneyin.",
                                "Ürün giriş uyarısı", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            //CurrentProduct.Id = Guid.NewGuid();

            //ObjectCtx.Context.AddToProducts(CurrentProduct);
            //ObjectCtx.Context.SaveChanges();
            //DialogResult = true;
            Close();
        }

        /// <summary>
        /// Selects the content of the textbox for easy editing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t == null)
            {
                return;
            }
            t.SelectAll();
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                return;
            }

            // Barcode machines simulate the return key after writing down the barcode.
            // Prevent displaying incomplete entry message
            e.Handled = true;
            nameTextBox.Focus();
        }
    }
}
