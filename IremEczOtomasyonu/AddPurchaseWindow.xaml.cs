﻿using System;
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
using IremEczOtomasyonu.BL;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddPurchaseWindow : Window
    {
        //private readonly Model1Container _dbContext;
        public ProductPurchase CurrentPurchase { get; private set; }
        public Product CurrentProduct { get; private set; }
        private readonly ExpirationDate _currExpirationDate;

        public AddPurchaseWindow(Product product)
        {
            InitializeComponent();
            CurrentProduct = product;

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

            // check the curr. expiration date in db
            ExpirationDate expirationDate = ObjectCtx.Context.ExpirationDates.FirstOrDefault(
                x => x.ProductId == CurrentProduct.Id && x.ExDate == _currExpirationDate.ExDate);
            if (expirationDate == null)
            {
                _currExpirationDate.NumItems = CurrentPurchase.NumItems;
                _currExpirationDate.Id = Guid.NewGuid();
                ObjectCtx.Context.AddToExpirationDates(_currExpirationDate);
            }
            else
            {
                expirationDate.NumItems += CurrentPurchase.NumItems;
            }

            CurrentPurchase.Id = Guid.NewGuid();
            CurrentPurchase.Product.NumItems += CurrentPurchase.NumItems;
            ObjectCtx.Context.AddToProductPurchases(CurrentPurchase);

            ObjectCtx.Context.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void ExpirationDateDatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            UIUtilities.DatePickerSelectDecade(sender as DatePicker);
        }
    }
}
