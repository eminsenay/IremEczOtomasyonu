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

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddNewProductWindow : Window
    {
        private Model1Container _dbContext;
        public Product CurrentProduct { get; private set; }

        public AddNewProductWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dbContext = new Model1Container();
            ObjectDataProvider currentProductDataProvider = ((ObjectDataProvider) (FindResource("currentProduct")));
            CurrentProduct = currentProductDataProvider.Data as Product;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            //BindingExpression expression = barcodeTextBox.GetBindingExpression(TextBox.TextProperty);
            //if (expression != null)
            //{
            //    expression.UpdateSource();
            //}
            if (Validation.GetHasError(barcodeTextBox) || Validation.GetHasError(nameTextBox) ||
                Validation.GetHasError(brandTextBox) || Validation.GetHasError(numItemsInStockTextBox) ||
                Validation.GetHasError(currentBuyingPriceTextBox) || Validation.GetHasError(currentSellingPriceTextBox))
            {
                MessageBox.Show("Girdiğiniz bazı bilgiler eksik ya da hatalı. \n Lütfen düzeltip tekrar deneyin.",
                                "Ürün giriş uyarısı", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Product lastProduct = _dbContext.Products.OrderByDescending(o => o.Id).FirstOrDefault();
            long productId = lastProduct != null ? lastProduct.Id + 1 : 1;
            CurrentProduct.Id = productId;

            _dbContext.AddToProducts(CurrentProduct);
            _dbContext.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}
