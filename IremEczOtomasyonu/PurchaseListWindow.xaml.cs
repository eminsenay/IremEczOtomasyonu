﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// Interaction logic for PurchaseListWindow.xaml
    /// </summary>
    public partial class PurchaseListWindow : Window
    {
        /// <summary>
        /// Flag for manual edit commits. With this flag, manual edit commit method doesn't cause an infinite loop.
        /// </summary>
        private bool _isManualEditCommit;
        private ObservableCollection<ProductPurchase> _productPurchases;
        private readonly List<ProductPurchase> _changedProductPurchases;

        public PurchaseListWindow()
        {
            InitializeComponent();
            _changedProductPurchases = new List<ProductPurchase>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource productPurchasesViewSource = ((CollectionViewSource)(
                FindResource("productPurchasesViewSource")));
            _productPurchases = new ObservableCollection<ProductPurchase>(ObjectCtx.Context.ProductPurchases.OrderBy(
                x => x.PurchaseDate));
            // Set prev num items by hand to keep track of numitems modifications
            foreach (ProductPurchase productPurchase in _productPurchases)
            {
                productPurchase.PrevNumItems = productPurchase.NumItems;
            }
            productPurchasesViewSource.Source = _productPurchases;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: validation check

            ObjectCtx.Context.SaveChanges();

            foreach (ProductPurchase productPurchase in _changedProductPurchases)
            {
                DataGridRow dataGridRow = productPurchasesDataGrid.ItemContainerGenerator.ContainerFromItem(
                    productPurchase) as DataGridRow;
                if (dataGridRow == null)
                {
                    continue;
                }
                dataGridRow.Background = Brushes.White;
            }

            _changedProductPurchases.Clear();

            applyButton.IsEnabled = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: validation check

            ObjectCtx.Context.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void ProductPurchasesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_isManualEditCommit)
            {
                return;
            }
            _isManualEditCommit = true;
            productPurchasesDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            ProductPurchase currProductPurchase = productPurchasesDataGrid.SelectedItem as ProductPurchase;
            if (currProductPurchase == null)
            {
                Debug.Fail("Current product sale could not be retrieved.");
                return;
            }
            _changedProductPurchases.Add(currProductPurchase);
            applyButton.IsEnabled = true;

            // Set the product and expiration date number of items.
            if (currProductPurchase.PrevNumItems != currProductPurchase.NumItems)
            {
                int diff = currProductPurchase.NumItems - currProductPurchase.PrevNumItems;

                currProductPurchase.Product.NumItems += diff;
                ExpirationDate selectedExpDate = currProductPurchase.Product.ExpirationDates.FirstOrDefault(
                    x => x.ExDate == currProductPurchase.ExDate);
                //Debug.Assert(selectedExpDate != null, "Expiration Date of the sale item cannot be found.");
                if (selectedExpDate != null)
                {
                    selectedExpDate.NumItems += diff;
                }
                else
                {
                    // User has modified the product's expiration date after purchasing it
                    // We don't track expiration date modifications, so create a new expirationDate object with 
                    // the original date and notify the user.

                    MessageBox.Show(this, "Ürünü sisteme girdikten sonra son kullanma tarihlerini değiştirmişsiniz.\n" +
                        "Eklenen/çıkarılan ürünler yine de orijinal son kullanma tarihini kullanacaklardır.\n" +
                        "Bu penceredeki işinizin bitiminden sonra ürün detaylarına girip son kullanma tarihlerini " +
                        "kontrol etmeniz faydalı olabilir.", "Ürün son kullanma tarihi uyarısı", MessageBoxButton.OK);
                    ExpirationDate orgExpirationDate = new ExpirationDate
                    {
                        Id = Guid.NewGuid(),
                        ExDate = currProductPurchase.ExDate,
                        NumItems = diff,
                        Product = currProductPurchase.Product
                    };
                    currProductPurchase.Product.ExpirationDates.Add(orgExpirationDate);
                }
                currProductPurchase.PrevNumItems = currProductPurchase.NumItems;
            }

            // Change bg color
            DataGridRow dataGridRow = productPurchasesDataGrid.ItemContainerGenerator.ContainerFromIndex(
                productPurchasesDataGrid.SelectedIndex) as DataGridRow;
            if (dataGridRow == null)
            {
                return;
            }
            dataGridRow.Background = Brushes.Aquamarine;
        }
    }
}
