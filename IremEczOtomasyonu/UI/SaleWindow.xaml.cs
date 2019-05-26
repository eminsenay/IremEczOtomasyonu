using System;
using System.Windows;
using IremEczOtomasyonu.Models;
using IremEczOtomasyonu.BL;
using System.Collections.Generic;

namespace IremEczOtomasyonu.UI
{
    /// <summary>
    /// Interaction logic for SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        //private readonly Model1Container _dbContext;

        /// <summary>
        /// Initializes a new sale window
        /// </summary>
        public SaleWindow()
        {
            InitializeComponent();
            
            userControlSales.CurrentProductSale = new ProductSale
                                                  {
                                                      SaleItems = new List<SaleItem>(),
                                                      SaleDate = DateTime.Now,
                                                      TotalPrice = 0
                                                  };
            userControlSales.saleGrid.DataContext = userControlSales.CurrentProductSale;

            userControlSales.barcodeTextBox.Focus();
        }

        /// <summary>
        /// Initializes a new sale window for the given customer.
        /// </summary>
        /// <param name="customer">Customer who is buying the products</param>
        public SaleWindow(Customer customer): this()
        {
            userControlSales.CurrentProductSale.Customer = customer;
        }


        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // validation check
            string validationMsg = userControlSales.Validate();
            if (validationMsg != null)
            {
                MessageBox.Show(validationMsg, "Satış uyarısı", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string stockControlMsg = userControlSales.StockControl();
            if (stockControlMsg != null)
            {
                MessageBox.Show(stockControlMsg, "Satış uyarısı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            if (userControlSales.CurrentProductSale.SaleItems.Count == 0)
            {
                DialogResult = true;
                Close();
                return;
            }

            userControlSales.CurrentProductSale.Id = Guid.NewGuid();

            ObjectCtx.Context.ProductSales.Add(userControlSales.CurrentProductSale);
            ObjectCtx.Context.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DialogResult != true)
            {
                // Remove all added sale items.
                userControlSales.RevertProductSale();
            }
        }
    }
}
