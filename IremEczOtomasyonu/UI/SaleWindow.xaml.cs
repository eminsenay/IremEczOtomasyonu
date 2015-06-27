using System;
using System.Data.Objects.DataClasses;
using System.Windows;
using IremEczOtomasyonu.BL;

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
                                                      SaleItems = new EntityCollection<SaleItem>(),
                                                      SaleDate = DateTime.Now,
                                                      TotalPrice = 0
                                                  };
            userControlSales.saleGrid.DataContext = userControlSales.CurrentProductSale;

            userControlSales.barcodeTextBox.Focus();

            // Attach the product sale to the context so that detach actions on cancel don't cause problems 
            // Without the line an exception is fired if the user cancels before adding items to the sale.
            ObjectCtx.Context.AddToProductSales(userControlSales.CurrentProductSale);
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
