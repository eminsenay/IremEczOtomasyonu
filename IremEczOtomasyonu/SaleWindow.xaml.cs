using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IremEczOtomasyonu.BL;

namespace IremEczOtomasyonu
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
                                                      SaleItems = new EntityCollection<SaleItem>(),
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
            
            if (userControlSales.CurrentProductSale.SaleItems.Count == 0)
            {
                // detach the current product sale since window is closed without any actual sale
                ObjectCtx.Context.Detach(userControlSales.CurrentProductSale);
                DialogResult = true;
                Close();
                return;
            }

            userControlSales.CurrentProductSale.Id = Guid.NewGuid();

            ObjectCtx.Context.AddToProductSales(userControlSales.CurrentProductSale);
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
                // detach the current product sale since window is closed without any actual sale
                ObjectCtx.Context.Detach(userControlSales.CurrentProductSale);
            }
        }
    }
}
