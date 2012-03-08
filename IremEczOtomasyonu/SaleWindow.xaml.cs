using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Threading;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        private readonly Model1Container _dbContext;
        private readonly Customer _customer;
        public List<SaleItem> SaleItems { get; set; }
        private ICollectionView CurrentView { get; set; }
        private bool _isManualEditCommit;

        public SaleWindow(Model1Container dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            SaleItems = new List<SaleItem>();

            CollectionViewSource saleItemsViewSource = ((CollectionViewSource)(FindResource("saleItemsViewSource")));
            saleItemsViewSource.Source = SaleItems;
            CurrentView = saleItemsViewSource.View;
        }

        public SaleWindow(Model1Container dbContext, Customer customer): this(dbContext)
        {
            _customer = customer;
        }

        private void productSaleDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //if (e.Column == barcodeColumn)
            //{
            //    TextBox barcodeTextBox = e.EditingElement as TextBox;
            //    SaleItem currItem = productSaleDataGrid.CurrentItem as SaleItem;
            //    if (barcodeTextBox == null || currItem == null)
            //    {
            //        return;
            //    }
            //    Product product = _dbContext.Products.First(p => p.Barcode == barcodeTextBox.Text);
            //    if (product != null)
            //    {
            //        currItem.Product = product;
            //    }
            //}
            //// TODO: Combobox
            //BindingExpression expression = e.EditingElement.GetBindingExpression(TextBox.TextProperty);
            //if (expression != null)
            //{
            //    expression.UpdateSource();
            //}
            //Dispatcher.BeginInvoke(
            //    new Action(() => productSaleDataGrid.Items.Refresh()), DispatcherPriority.Background);
        }

        private void productSaleDataGrid_CurrentCellChanged(object sender, System.EventArgs e)
        {
            //if (rowBeingEdited != null)
            //{
            //    rowBeingEdited.EndEdit();
            //}
        }

        private void productAddButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: sanity check
            Product newProduct = _dbContext.Products.First(p => p.Barcode == barcodeTextBox.Text);
            if (newProduct != null)
            {
                SaleItems.Add(new SaleItem {Product = newProduct, NumSold = 1});
                CurrentView.Refresh();
            }
            // TODO: Messagebox if not found
        }
    }
}
