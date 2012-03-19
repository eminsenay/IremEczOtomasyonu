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
        private readonly Model1Container _dbContext;
        public ProductPurchase CurrentPurchase { get; private set; }
        public Product CurrentProduct { get; private set; }
        private readonly ExpirationDate _currExpirationDate;

        public AddPurchaseWindow(Product product, Model1Container dbContext)
        {
            InitializeComponent();
            CurrentProduct = product;
            _dbContext = dbContext;

            _currExpirationDate = new ExpirationDate { ProductId = CurrentProduct.Id };
            expirationDateDatePicker.DataContext = _currExpirationDate;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource productPurchasesViewSource = ((CollectionViewSource)
                (FindResource("productPurchasesViewSource")));
            
            CurrentPurchase = new ProductPurchase
                               {
                                   Product = CurrentProduct,
                                   ProductId = CurrentProduct.Id,
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

            // check the curr. expiration date in db
            ExpirationDate expirationDate = _dbContext.ExpirationDates.FirstOrDefault(
                x => x.ProductId == CurrentProduct.Id && x.ExDate == _currExpirationDate.ExDate);
            if (expirationDate == null)
            {
                ExpirationDate lastExpDate =
                    _dbContext.ExpirationDates.OrderByDescending(x => x.Id).FirstOrDefault();
                long expDateId = lastExpDate != null ? lastExpDate.Id + 1 : 1;

                _currExpirationDate.NumItems = CurrentPurchase.NumItems;
                _currExpirationDate.Id = expDateId;
                _dbContext.AddToExpirationDates(_currExpirationDate);
            }
            else
            {
                expirationDate.NumItems += CurrentPurchase.NumItems;
            }

            CurrentPurchase.Id = purchaseId;
            CurrentPurchase.Product.NumItems += CurrentPurchase.NumItems;
            _dbContext.AddToProductPurchases(CurrentPurchase);

            _dbContext.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void ExpirationDateDatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            Utilities.DatePickerSelectDecade(sender as DatePicker);
        }
    }
}
