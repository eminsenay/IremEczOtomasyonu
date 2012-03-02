using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
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
    public partial class AddPurchaseWindow : Window
    {
        private Model1Container _dbContext;
        public ProductPurchase CurrentPurchase { get; private set; }

        public AddPurchaseWindow(string barcode)
        {
            InitializeComponent();
            barcodeTextBox.Text = barcode;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dbContext = new Model1Container();
            CollectionViewSource productPurchasesViewSource = ((CollectionViewSource)
                (FindResource("productPurchasesViewSource")));
            Product product = _dbContext.Products.First(p => p.Barcode == barcodeTextBox.Text);
            CurrentPurchase = new ProductPurchase
                               {
                                   Product = product,
                                   ProductId = product.Id,
                                   PurchaseDate = DateTime.Now,
                               };
            productPurchasesViewSource.Source = new List<ProductPurchase> { CurrentPurchase };
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(numItemsBoughtTextBox) || Validation.GetHasError(priceTextBox) || 
                Validation.GetHasError(purchaseDateDatePicker) || Validation.GetHasError(expirationDateDatePicker) ||
                Validation.GetHasError(remarksTextBox))
            {
                MessageBox.Show("Girdiğiniz bazı bilgiler eksik ya da hatalı. \n Lütfen düzeltip tekrar deneyin.",
                                "Ürün alımı uyarısı", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CurrentPurchase.Price > 0 && CurrentPurchase.Price != CurrentPurchase.Product.CurrentBuyingPrice)
            {
                MessageBoxResult res = MessageBox.Show(this, 
                    "Girdiğiniz alış fiyatı ürünün güncel alış fiyatından farklı. \nGüncel alış fiyatını değiştirmek ister misiniz?", 
                    "Güncel Alış Fiyatı Uyarısı", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Cancel)
                {
                    return;
                }
                if (res == MessageBoxResult.Yes)
                {
                    CurrentPurchase.Product.CurrentBuyingPrice = CurrentPurchase.Price;
                }
            }

            ProductPurchase lastPurchase = _dbContext.ProductPurchases.OrderByDescending(pu => pu.Id).FirstOrDefault();
            long purchaseId = lastPurchase != null ? lastPurchase.Id + 1 : 1;

            CurrentPurchase.Id = purchaseId;
            CurrentPurchase.Product.NumItems += CurrentPurchase.NumItems;
            _dbContext.AddToProductPurchases(CurrentPurchase);

            _dbContext.SaveChanges();
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Opens the decade mode for easy selection of the expiration date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpirationDateDatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            var datepicker = sender as DatePicker;
            if (datepicker == null)
            {
                return;
            }
            var popup = datepicker.Template.FindName("PART_Popup", datepicker) as Popup;
            if (popup != null && popup.Child is Calendar)
            {
                ((Calendar)popup.Child).DisplayMode = CalendarMode.Decade;
            }
        }
    }
}
