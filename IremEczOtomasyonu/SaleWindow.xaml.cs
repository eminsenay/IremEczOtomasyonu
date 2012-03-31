using System;
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
        private readonly Model1Container _dbContext;

        /// <summary>
        /// Initializes a new sale window
        /// </summary>
        /// <param name="dbContext">Database context to use.</param>
        public SaleWindow(Model1Container dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            
            userControlSales.DbContext = dbContext;
            userControlSales.CurrentProductSale = new ProductSale
                                                  {
                                                      SaleItems = new EntityCollection<SaleItem>(),
                                                      SaleDate = DateTime.Now,
                                                      TotalPrice = 0
                                                  };

        }

        /// <summary>
        /// Initializes a new sale window for the given customer.
        /// </summary>
        /// <param name="dbContext">Database context to use.</param>
        /// <param name="customer">Customer who is buying the products</param>
        public SaleWindow(Model1Container dbContext, Customer customer): this(dbContext)
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
                DialogResult = true;
                Close();
                return;
            }

            userControlSales.CurrentProductSale.Id = Guid.NewGuid();

            // Set entity expiration dates and ids manually
            foreach (SaleItem saleItem in userControlSales.CurrentProductSale.SaleItems)
            {
                saleItem.ExDate = saleItem.ExpDate.ExDate;
                // Decrease the item count from products & expiration date tables
                saleItem.Product.NumItems -= saleItem.NumSold;
                saleItem.ExpDate.NumItems -= saleItem.NumSold;
                saleItem.Id = Guid.NewGuid();
            }
            
            _dbContext.AddToProductSales(userControlSales.CurrentProductSale);
            _dbContext.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}
