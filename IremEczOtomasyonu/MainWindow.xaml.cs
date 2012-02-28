using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IremEczOtomasyonu.Model1Container _dbContext;
        private System.Windows.Data.CollectionViewSource _customersViewSource;
        private bool _allChangesSaved = true;
        private bool _customerModifiable = false;
        private Brush _defaultTextFgColor = Brushes.Gray;
        private List<Customer> Customers { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeCustomerSearchControls();
            //Focus the grid to receive the changed events of the selected customer correctly
            customersDataGrid.Focus();

            _dbContext = new IremEczOtomasyonu.Model1Container();

            _customersViewSource = ((System.Windows.Data.CollectionViewSource)(
                this.FindResource("customersViewSource")));
            System.Data.Objects.ObjectQuery<IremEczOtomasyonu.Customer> customersQuery =
                this.GetCustomersQuery(_dbContext);
            Customers = customersQuery.Execute(System.Data.Objects.MergeOption.AppendOnly).ToList();
            _customersViewSource.Source = Customers;
        }

        private void InitializeCustomerSearchControls()
        {
            searchFirstNameTextBox.Text = Resources1.FirstName;
            searchLastNameTextBox.Text = Resources1.LastName;
            searchDetailedInfoTextBox.Text = Resources1.DetailedInfo;
            searchFirstNameTextBox.Foreground = _defaultTextFgColor;
            searchLastNameTextBox.Foreground = _defaultTextFgColor;
            searchDetailedInfoTextBox.Foreground = _defaultTextFgColor;
        }

        private void CustomerSearchControl_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                Debug.Fail("Sender of the gotFocus method is not a TextBox.");
                return;
            }

            if (textBox.Foreground == _defaultTextFgColor)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        private void CustomerSearchControl_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                Debug.Fail("Sender of the gotFocus method is not a TextBox.");
                return;
            }

            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Foreground = _defaultTextFgColor;

                string defaultText = string.Empty;
                if (textBox == searchFirstNameTextBox)
                {
                    defaultText = Resources1.FirstName;
                }
                else if (textBox == searchLastNameTextBox)
                {
                    defaultText = Resources1.LastName;
                }
                else if (textBox == searchDetailedInfoTextBox)
                {
                    defaultText = Resources1.DetailedInfo;
                }
                textBox.Text = defaultText;
            }
        }

        private System.Data.Objects.ObjectQuery<Customer> GetCustomersQuery(Model1Container model1Container)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<IremEczOtomasyonu.Customer> customersQuery = model1Container.Customers;
            // Update the query to include Purchases data in Customers. You can modify this code as needed.
            customersQuery = customersQuery.Include("ProductSales");
            // Returns an ObjectQuery.
            return customersQuery;
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = searchFirstNameTextBox.Foreground == _defaultTextFgColor ? null : 
                searchFirstNameTextBox.Text;
            string lastName = searchLastNameTextBox.Foreground == _defaultTextFgColor ? null : 
                searchLastNameTextBox.Text;
            string detailedInfo = searchDetailedInfoTextBox.Foreground == _defaultTextFgColor ? null : 
                searchDetailedInfoTextBox.Text;
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Lütfen bir isim ve soyisim girin.");
                return;
            }
            Customer newCustomer = new Customer { FirstName = firstName, LastName = lastName, 
                DetailedInfo = detailedInfo };
            Customer lastCustomer = _dbContext.Customers.OrderByDescending(o => o.Id).FirstOrDefault();

            newCustomer.Id = lastCustomer == null ? 1 : lastCustomer.Id + 1;
            Customers.Add(newCustomer);
            _dbContext.AddToCustomers(newCustomer);
            _dbContext.SaveChanges();
            _customersViewSource.View.Refresh();
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            _dbContext.SaveChanges();
            // Use the views current item instead of selected item because of the last empty row.
            DataGridRow selectedRow = customersDataGrid.ItemContainerGenerator.ContainerFromIndex(
                customersDataGrid.Items.IndexOf(_customersViewSource.View.CurrentItem)) as DataGridRow;
            selectedRow.Background = Brushes.White;
            _allChangesSaved = true;
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
            if (fileSelected == null || fileSelected == false)
            {
                return;
            }

            string imagePath = openFileDialog.FileName;
            Customer currCustomer = _customersViewSource.View.CurrentItem as Customer;
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
            if (selectedCustomer == null && _customersViewSource != null)
            {
                // If the last empty row is selected, details keep displaying the last selected item.
                selectedCustomer = _customersViewSource.View.CurrentItem as Customer;
            }
            ShowCustomerPhoto(selectedCustomer);
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
                addPhotoButton.Visibility = System.Windows.Visibility.Visible;
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
            addPhotoButton.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SelectedCustomerModified(object sender, TextChangedEventArgs e)
        {
            OnSelectedCustomerModified();
        }

        private void CustomersDataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            _customerModifiable = true;
        }

        private void CustomersDataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            _customerModifiable = false;
        }

        private void BirthdayDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectedCustomerModified();
        }

        /// <summary>
        /// Checks if the customer can be modified. If so, it changes the background of the selected row in the 
        /// customers datagrid to display that it is not saved yet.
        /// </summary>
        private void OnSelectedCustomerModified()
        {
            if (!_customerModifiable)
            {
                return;
            }
            _allChangesSaved = false;
            // Use the views current item instead of selected item because of the last empty row.
            DataGridRow dataGridRow = customersDataGrid.ItemContainerGenerator.ContainerFromIndex(
                customersDataGrid.Items.IndexOf(_customersViewSource.View.CurrentItem)) as DataGridRow;
            if (dataGridRow == null)
            {
                return;
            }
            dataGridRow.Background = Brushes.Aquamarine;
        }

        /// <summary>
        /// On a close request, checks if any unsaved changes exist and warns the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_allChangesSaved)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show(
                "Kaydedilmemiş değişiklikleriniz var. Yine de çıkmak istiyor musunuz?", 
                "Uyarı", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void PhotoImgDeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Resmi silmek istediğinizden emin misiniz?", "Resim Silme Onayı",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            
            Customer currCustomer = _customersViewSource.View.CurrentItem as Customer;
            if (currCustomer == null)
            {
                return;
            }

            // Set customerModifiable manually to true since clicking the context menu item doesn't change the focus and
            // it causes problems if the focus is in the datagrid.
            _customerModifiable = true;
            currCustomer.Photo = null;
            ShowCustomerPhoto(currCustomer);
            OnSelectedCustomerModified();
        }

        private void CustomerSearchControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_customersViewSource == null || _customersViewSource.View == null)
            {
                return;
            }
            _customersViewSource.View.Refresh();
        }

        private void CustomerCollection_Filter(object sender, FilterEventArgs e)
        {
            Customer c = e.Item as Customer;
            if (c == null || searchFirstNameTextBox == null || searchLastNameTextBox == null || 
                searchDetailedInfoTextBox == null)
            {
                e.Accepted = true;
                return;
            }

            string firstName = searchFirstNameTextBox.Foreground == _defaultTextFgColor ? string.Empty : 
                searchFirstNameTextBox.Text.ToUpperInvariant();
            string lastName = searchLastNameTextBox.Foreground == _defaultTextFgColor ? string.Empty : 
                searchLastNameTextBox.Text.ToUpperInvariant();
            string detailedInfo = searchDetailedInfoTextBox.Foreground == _defaultTextFgColor ? string.Empty :
                searchDetailedInfoTextBox.Text.ToUpperInvariant();

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
        /// <param name="currCustomer"></param>
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
                _customersViewSource.View.Refresh();
            }
            return;
        }

        private void DatagridDeleteCustomerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedCustomer();
        }
    }
}
