using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        
        private bool _isManualEditCommit;
        //private bool _refreshRequired;

        //public ObservableCollection<SaleItem> SaleItems { get; set; }
        public MasterSaleItem MSaleItem { get; set; }

        //private ICollectionView CurrentView { get; set; }

        public SaleWindow(Model1Container dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            //SaleItems = new ObservableCollection<SaleItem>();

            MSaleItem = new MasterSaleItem();
            //CollectionViewSource saleItemsViewSource = ((CollectionViewSource)(FindResource("saleItemsViewSource")));
            //saleItemsViewSource.Source = SaleItems;
            //saleItemsViewSource.Source = MSaleItem;

            productSaleDataGrid.ItemsSource = MSaleItem.SaleItems;
            //totalPriceTextBlock.DataContext = MSaleItem.TotalPrice;
            //Binding binding = new Binding { Source = MSaleItem.TotalPrice };
            //totalPriceTextBlock.SetBinding(TextBlock.TextProperty, binding);
            totalPriceTextBlock.Text = "0";
            //Binding binding = new Binding
            //                  {Source = SaleItems, Converter = new ProductCollectionToTotalPriceConverter()};
            //totalPriceTextBlock.SetBinding(TextBlock.TextProperty, binding);

            //totalPriceTextBlock.DataContext = SaleItems;

            //CurrentView = saleItemsViewSource.View;

            //var query = SaleItems.Sum(x => x.NumSold*x.Price);
            //totalPriceTextBlock.DataContext = query;
        }

        public SaleWindow(Model1Container dbContext, Customer customer): this(dbContext)
        {
            _customer = customer;
            customerNameTextBlock.Text = _customer.FirstName + " " + _customer.LastName;
        }

        private void ProductAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (barcodeTextBox == null || string.IsNullOrEmpty(barcodeTextBox.Text))
            {
                return;
            }
            Product newProduct = _dbContext.Products.FirstOrDefault(p => p.Barcode == barcodeTextBox.Text);
            if (newProduct == null)
            {
                MessageBox.Show("Girmiş olduğunuz barkod sistemde kayıtlı değil.", "Ürün uyarısı", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                barcodeTextBox.SelectAll();
                return;
            }
            //SaleItems.Add(new SaleItem { Product = newProduct, NumSold = 1 });
            MSaleItem.SaleItems.Add(new SaleItem { Product = newProduct, NumSold = 1 });
            //MSaleItem.TotalPrice = MSaleItem.SaleItems.Sum(x => x.Price);
            totalPriceTextBlock.Text = MSaleItem.SaleItems.Sum(x => x.Price).ToString();
            //CurrentView.Refresh();
            barcodeTextBox.Text = string.Empty;
            barcodeTextBox.Focus();
        }

        /// <summary>
        /// CellEditEnding event handler of the datagrid. Commits the change and signals for the view refresh.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductSaleDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_isManualEditCommit)
            {
                return;
            }
            _isManualEditCommit = true;
            productSaleDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            _isManualEditCommit = false;
            totalPriceTextBlock.Text = MSaleItem.SaleItems.Sum(x => x.Price).ToString();
            //MSaleItem.TotalPrice = MSaleItem.SaleItems.Sum(x => x.Price);
            //_refreshRequired = true;
        }

        /// <summary>
        /// CurrentCellChanged event handler of the datagrid. Refreshes the view to update the total price if the 
        /// number of items are changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductSaleDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            //if (_refreshRequired)
            //{
            //    CurrentView.Refresh();
            //    _refreshRequired = false;
            //}
        }
    }

    public class MasterSaleItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get { return _totalPrice; }
            set 
            { 
                _totalPrice = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TotalPrice"));
            }
            //get { return SaleItems.Sum(x => x.Price); }
            //get { return 3; }
        }

        public ObservableCollection<SaleItem> SaleItems { get; set; }

        public MasterSaleItem()
        {
            SaleItems = new ObservableCollection<SaleItem>();
            SaleItems.CollectionChanged += SaleItemsOnCollectionChanged;
        }

        private void SaleItemsOnCollectionChanged(object sender, 
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            TotalPrice = SaleItems.Sum(x => x.Price);
            OnPropertyChanged(new PropertyChangedEventArgs("SaleItems"));
            OnPropertyChanged(new PropertyChangedEventArgs("TotalPrice"));
        }
    }
}
