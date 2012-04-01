using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
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
    /// Interaction logic for SaleListWindow.xaml
    /// </summary>
    public partial class SaleListWindow : Window
    {
        private readonly Model1Container _dbContext;

        public SaleListWindow(Model1Container dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            userControlSales.DbContext = _dbContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load data into ProductSales. You can modify this code as needed.
            CollectionViewSource productSalesViewSource = ((CollectionViewSource)(FindResource("productSalesViewSource")));
            productSalesViewSource.Source = _dbContext.ProductSales.Execute(
                System.Data.Objects.MergeOption.AppendOnly);

            Binding binding = new Binding
                              {
                                  Source = userControlSales.CurrentProductSale, Mode = BindingMode.OneWayToSource
                              };
            productSalesDataGrid.SetBinding(Selector.SelectedItemProperty, binding);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DialogResult != true)
            {
                // detach the current product sale since window is closed without any actual sale
                _dbContext.Detach(userControlSales.CurrentProductSale);
            }
        }
    }
}
