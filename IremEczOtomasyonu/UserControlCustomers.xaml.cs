
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using IremEczOtomasyonu.BL;
using Microsoft.Win32;
using System.Diagnostics;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for UserControlCustomers.xaml
    /// </summary>
    public partial class UserControlCustomers : UserControl
    {
        private readonly Model1Container _dbContext;
        private ICollectionView _customerView;
        private ICollectionView _saleItemsView;

        public bool AllChangesSaved { get; private set; }
        private ObservableCollection<Customer> Customers { get; set; }
        private ObservableCollection<SaleItem> _saleItems;

        public UserControlCustomers(Model1Container dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource customersViewSource = ((CollectionViewSource)(FindResource("customersViewSource")));
            Customers = new ObservableCollection<Customer>(_dbContext.Customers);
            customersViewSource.Source = Customers;
            _customerView = customersViewSource.View;
            AllChangesSaved = true;

            CollectionViewSource saleItemsViewSource = ((CollectionViewSource)(FindResource("saleItemsViewSource")));
            //saleItemsViewSource.Source = _dbContext.SaleItems.Execute(MergeOption.AppendOnly);
            _saleItems = new ObservableCollection<SaleItem>(_dbContext.SaleItems.Execute(MergeOption.AppendOnly));
            saleItemsViewSource.Source = _saleItems;
            _saleItemsView = saleItemsViewSource.View;

            _saleItemsView.GroupDescriptions.Add(new PropertyGroupDescription("ProductSale.Id"));
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = searchFirstNameInfoTextBox.Text;
            string lastName = searchLastNameInfoTextBox.Text;
            string detailedInfo = searchDetailedInfoInfoTextBox.Text;
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Lütfen bir isim ve soyisim girin.");
                return;
            }
            Customer newCustomer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                DetailedInfo = detailedInfo
            };
            
            newCustomer.Id = Guid.NewGuid();
            Customers.Add(newCustomer);
            _dbContext.AddToCustomers(newCustomer);
            _dbContext.SaveChanges();
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            _dbContext.SaveChanges();
            DataGridRow selectedRow = customersDataGrid.ItemContainerGenerator.ContainerFromIndex(
                customersDataGrid.SelectedIndex) as DataGridRow;
            if (selectedRow == null)
            {
                return;
            }
            selectedRow.Background = Brushes.White;
            AllChangesSaved = true;
        }

        /// <summary>
        /// Adds a new photo to a customer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? fileSelected = openFileDialog.ShowDialog();
            if (fileSelected == false)
            {
                return;
            }

            string imagePath = openFileDialog.FileName;
            Customer currCustomer = customersDataGrid.SelectedItem as Customer;
            if (currCustomer == null)
            {
                return;
            }

            currCustomer.Photo = ConvertImageToByteArray(imagePath);
            ShowCustomerPhoto(currCustomer);
            OnSelectedCustomerModified();
        }

        private static byte[] ConvertImageToByteArray(string fileName)
        {
            System.Drawing.Bitmap bitMap = new System.Drawing.Bitmap(fileName);
            System.Drawing.Imaging.ImageFormat bmpFormat = bitMap.RawFormat;
            var imageToConvert = System.Drawing.Image.FromFile(fileName);
            using (MemoryStream ms = new MemoryStream())
            {
                imageToConvert.Save(ms, bmpFormat);
                return ms.ToArray();
            }
        }

        private void CustomersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Customer selectedCustomer = (e.AddedItems == null || e.AddedItems.Count == 0) ? null :
                e.AddedItems[0] as Customer;
            if (selectedCustomer == null)
            {
                return;
            }
            ShowCustomerPhoto(selectedCustomer);
            if (_saleItemsView != null)
            {
                _saleItemsView.Refresh();
            }
        }

        /// <summary>
        /// Shows the given customer's photo in the details group. If the customer or its Photo property is null, 
        /// add photo button is displayed.
        /// </summary>
        /// <param name="customer"></param>
        private void ShowCustomerPhoto(Customer customer)
        {
            if (customer == null || customer.Photo == null)
            {
                // Empty the photoImage and show the AddPhoto Button
                photoImage.Source = null;
                addPhotoButton.Visibility = Visibility.Visible;
                return;
            }
            // Set the source of the photoImage
            using (var stream = new MemoryStream(customer.Photo))
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = stream;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                img.Freeze();
                photoImage.Source = img;
            }
            // Hide the addPhoto Button
            addPhotoButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Checks if the customer can be modified. If so, it changes the background of the selected row in the 
        /// customers datagrid to display that it is not saved yet.
        /// </summary>
        private void OnSelectedCustomerModified()
        {
            AllChangesSaved = false;
            DataGridRow dataGridRow = customersDataGrid.ItemContainerGenerator.ContainerFromIndex(
                customersDataGrid.SelectedIndex) as DataGridRow;
            if (dataGridRow == null)
            {
                return;
            }
            dataGridRow.Background = Brushes.Aquamarine;
        }

        private void PhotoImgDeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Resmi silmek istediğinizden emin misiniz?", "Resim Silme Onayı",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            Customer currCustomer = customersDataGrid.SelectedItem as Customer;
            if (currCustomer == null)
            {
                return;
            }

            currCustomer.Photo = null;
            ShowCustomerPhoto(currCustomer);
            OnSelectedCustomerModified();
        }

        private void CustomerSearchControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_customerView == null)
            {
                return;
            }
            _customerView.Refresh();
            if (_saleItemsView != null)
            {
                _saleItemsView.Refresh();
            }
        }

        private void CustomerCollection_Filter(object sender, FilterEventArgs e)
        {
            Customer c = e.Item as Customer;
            if (c == null || searchFirstNameInfoTextBox == null || searchLastNameInfoTextBox == null ||
                searchDetailedInfoInfoTextBox == null)
            {
                e.Accepted = true;
                return;
            }
            
            string firstName = searchFirstNameInfoTextBox.Text.ToUpperInvariant();
            string lastName = searchLastNameInfoTextBox.Text.ToUpperInvariant();
            string detailedInfo = searchDetailedInfoInfoTextBox.Text.ToUpperInvariant();

            string customerFirstName = c.FirstName == null ? string.Empty : c.FirstName.ToUpperInvariant();
            string customerLastName = c.LastName == null ? string.Empty : c.LastName.ToUpperInvariant();
            string customerDetailedInfo = c.DetailedInfo == null ? string.Empty : c.DetailedInfo.ToUpperInvariant();

            if (customerFirstName.Contains(firstName) && customerLastName.Contains(lastName) &&
                customerDetailedInfo.Contains(detailedInfo))
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
        }

        private void CustomersDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteSelectedCustomer();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Asks and deletes the selected customer of the datagrid.
        /// </summary>
        private void DeleteSelectedCustomer()
        {
            Customer currCustomer = customersDataGrid.SelectedItem as Customer;
            if (currCustomer == null)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show("Seçili müşteriyi silmek istediğinizden emin misiniz?",
                "Müşteri silme onayı", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Customers.Remove(currCustomer);
                _dbContext.DeleteObject(currCustomer);
                _dbContext.SaveChanges();
            }

            // TODO: Purchases of the customer
        }

        private void DatagridDeleteCustomerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedCustomer();
        }

        private void SelectedCustomerModified(object sender, DataTransferEventArgs e)
        {
            OnSelectedCustomerModified();
        }

        private void DatagridSaleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Customer currCustomer = customersDataGrid.SelectedItem as Customer;
            ExecuteCustomerSale(currCustomer);
        }

        private void SaleItems_Filter(object sender, FilterEventArgs e)
        {
            SaleItem s = e.Item as SaleItem;
            Customer selectedCustomer = customersDataGrid.SelectedItem as Customer;
            if (s == null || selectedCustomer == null)
            {
                e.Accepted = false;
                return;
            }
            if (s.ProductSale.Customer == selectedCustomer)
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
        }

        public void ExecuteCustomerSale(Customer customer)
        {
            SaleWindow saleWindow = new SaleWindow(_dbContext, customer)
            {
                Owner = Parent as Window,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (saleWindow.ShowDialog() == true)
            {
                // Refresh the sale items
                _saleItems.Clear();
                ObjectResult<SaleItem> objectResult = _dbContext.SaleItems.Execute(MergeOption.AppendOnly);
                foreach (SaleItem saleItem in objectResult)
                {
                    _saleItems.Add(saleItem);
                }
            }
        }
    }
}
