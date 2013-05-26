using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddNewProductWindow : Window
    {
        public Product CurrentProduct { get; private set; }

        public AddNewProductWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ObjectDataProvider currentProductDataProvider = ((ObjectDataProvider)(FindResource("currentProduct")));
            CurrentProduct = currentProductDataProvider.Data as Product;
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


            CurrentProduct.Id = Guid.NewGuid();

            ObjectCtx.Context.AddToProducts(CurrentProduct);
            ObjectCtx.Context.SaveChanges();
            DialogResult = true;
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
