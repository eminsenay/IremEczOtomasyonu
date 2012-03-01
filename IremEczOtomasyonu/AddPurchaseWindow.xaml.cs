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
    public partial class AddPurchaseWindow : Window
    {
        private Model1Container _dbContext;
        private ProductCollection Products { get; set; }

        public AddPurchaseWindow(string barcode)
        {
            InitializeComponent();
            barcodeTextBox.Text = barcode;
            purchaseDateDatePicker.SelectedDate = DateTime.Now;
        }

        private ObjectQuery<Product> GetProductsQuery(Model1Container model1Container)
        {
            // Auto generated code

            ObjectQuery<Product> productsQuery = model1Container.Products.Where(p => p.Barcode == barcodeTextBox.Text) 
                as ObjectQuery<Product>;
            // Returns an ObjectQuery.
            return productsQuery;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (_dbContext == null)
            {
                _dbContext = new Model1Container();
            }
            // Load data into Products. You can modify this code as needed.
            CollectionViewSource productsViewSource = ((CollectionViewSource)(FindResource("productsViewSource")));
            ObjectQuery<Product> productsQuery = GetProductsQuery(_dbContext);
            //var productsQuery = from p in _dbContext.Products where p.Barcode == barcodeTextBox.Text select p;
            Products = new ProductCollection(productsQuery, _dbContext);
            if (Products.Count == 0)
            {
                Products.Add(new Product());
            }
            //productsViewSource.Source = productsQuery.Execute(MergeOption.AppendOnly);
            productsViewSource.Source = Products;
            //ListCollectionView lcw = (ListCollectionView) productsViewSource.View;
            //Product pr = (Product) lcw.AddNew();
            //lcw.CommitNew();



            // Load data into ProductPurchases. You can modify this code as needed.
            //CollectionViewSource productPurchasesViewSource = ((CollectionViewSource)(FindResource("productPurchasesViewSource")));
            //ObjectQuery<ProductPurchase> productPurchasesQuery = GetProductPurchasesQuery(_dbContext);
            //productPurchasesViewSource.Source = productPurchasesQuery.Execute(MergeOption.AppendOnly);
        }

        //private ObjectQuery<ProductPurchase> GetProductPurchasesQuery(Model1Container model1Container)
        //{
        //    return null;
        //    // Auto generated code

        //    //ObjectQuery<ProductPurchase> productPurchasesQuery = model1Container.ProductPurchases;
        //    // To explicitly load data, you may need to add Include methods like below:
        //    // productPurchasesQuery = productPurchasesQuery.Include("ProductPurchases.Product").
        //    // For more information, please see http://go.microsoft.com/fwlink/?LinkId=157380
        //    // Returns an ObjectQuery.
        //    //return productPurchasesQuery;
        //}

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            string barcode = barcodeTextBox.Text;
            string productName = nameTextBox.Text;
            string brandName = brandTextBox.Text;
            decimal currBuyingPrice = decimal.Parse(currentBuyingPriceTextBox.Text);
            decimal currSellinPrice = decimal.Parse(currentSellingPriceTextBox.Text);

            int numBought = int.Parse(numItemsBoughtTextBox.Text);
            decimal buyingPrice = decimal.Parse(priceTextBox.Text);
            DateTime buyingDate = (purchaseDateDatePicker.SelectedDate != null) ? 
                (DateTime) purchaseDateDatePicker.SelectedDate : DateTime.Now;
            DateTime expirationDate = (expirationDateDatePicker.SelectedDate != null) ?
                (DateTime)expirationDateDatePicker.SelectedDate : DateTime.Now;
            string buyingRemarks = remarksTextBox.Text;

            Product lastProduct = _dbContext.Products.OrderByDescending(o => o.Id).FirstOrDefault();
            long productId = lastProduct != null ? lastProduct.Id + 1 : 1;
            //Customer lastCustomer = _dbContext.Customers.OrderByDescending(o => o.Id).FirstOrDefault();
            Product product = new Product
                        {
                            Id = productId,
                            Barcode = barcode,
                            Name = productName,
                            Brand = brandName,
                            CurrentBuyingPrice = currBuyingPrice,
                            CurrentSellingPrice = currSellinPrice, 
                            NumItems = numBought,
                        };
            _dbContext.AddToProducts(product);

            ProductPurchase lastPurchase = _dbContext.ProductPurchases.OrderByDescending(pu => pu.Id).FirstOrDefault();
            long purchaseId = lastPurchase != null ? lastPurchase.Id + 1 : 1;
            ProductPurchase purchase = new ProductPurchase
                                       {
                                           Id = purchaseId,
                                           ProductId = productId,
                                           NumItems = numBought,
                                           PurchaseDate = buyingDate,
                                           ExpirationDate = expirationDate,
                                           Price = buyingPrice,
                                           Remarks = buyingRemarks,
                                       };
            _dbContext.AddToProductPurchases(purchase);

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

        private void nameTextBox_Error(object sender, ValidationErrorEventArgs e)
        {

        }
    }
}
