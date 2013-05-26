
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Objects;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using IremEczOtomasyonu.BL;
using Microsoft.Win32;
using System.Diagnostics;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for UserControlCustomers.xaml
    /// </summary>
    public partial class UserControlCustomers : UserControl
    {
        private ICollectionView _customerView;
        private Window _parentWindow;

        private Window ParentWindow
        {
            get { return _parentWindow ?? (_parentWindow = Window.GetWindow(this)); }
        }
        public bool AllChangesSaved { get; private set; }
        private ObservableCollection<Customer> Customers { get; set; }


        public UserControlCustomers()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource customersViewSource = ((CollectionViewSource)(FindResource("customersViewSource")));
            Customers = new ObservableCollection<Customer>(ObjectCtx.Context.Customers);
            customersViewSource.Source = Customers;
            customersViewSource.SortDescriptions.Add(new SortDescription("FirstName", ListSortDirection.Ascending));
            _customerView = customersViewSource.View;
            AllChangesSaved = true;

            // Expiration date sanity check
            //foreach (Product product in ObjectCtx.Context.Products)
            //{
            //    int expSum = 0;
            //    List<ExpirationDate> expirationDates = new List<ExpirationDate>();
            //    foreach (ExpirationDate expirationDate in ObjectCtx.Context.ExpirationDates)
            //    {
            //        if (expirationDate.ProductId != product.Id)
            //        {
            //            continue;
            //        }
            //        expSum += expirationDate.NumItems;
            //        expirationDates.Add(expirationDate);
            //    }

            //    if (product.NumItems != expSum)
            //    {
            //        Debug.Fail("This should not happen");
            //    }
            //}
        }

        public void Reload()
        {
            Customers.Clear();
            foreach (Customer customer in ObjectCtx.Context.Customers)
            {
                Customers.Add(customer);
            }
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
                DetailedInfo = detailedInfo,
                Id = Guid.NewGuid()
            };
            
            Customers.Add(newCustomer);
            ObjectCtx.Context.AddToCustomers(newCustomer);
            ObjectCtx.Context.SaveChanges();

            // select the customer from the customers datagrid so that the user can enter additional details 
            // without problems
            customersDataGrid.SelectedItem = newCustomer;
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            ObjectCtx.Context.SaveChanges();
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

            Bitmap customerPhotoBitmap = new Bitmap(imagePath);
            currCustomer.Photo = ConvertBitmapToByteArray(customerPhotoBitmap);
            ShowCustomerPhoto(currCustomer);
            OnSelectedCustomerModified();
        }

        /// <summary>
        /// Captures a photo from the selected webcam.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CapturePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            WebcamWindow webcamWindow = new WebcamWindow { Owner = ParentWindow };
            bool? dialogResult = webcamWindow.ShowDialog();
            if (dialogResult != true)
            {
                return;
            }

            Customer currCustomer = customersDataGrid.SelectedItem as Customer;
            if (currCustomer == null)
            {
                return;
            }

            currCustomer.Photo = ConvertBitmapToByteArray(webcamWindow.GrabbedImage);
            ShowCustomerPhoto(currCustomer);
            OnSelectedCustomerModified();
        }

        private static byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                return memory.ToArray();
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
                // Empty the photoImage, show addPhoto and capturePhoto Buttons, show border
                photoImage.Source = null;
                addPhotoButton.Visibility = Visibility.Visible;
                capturePhotoButton.Visibility = Visibility.Visible;
                imageBorder.BorderThickness = new Thickness(1);
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
            // Hide the addPhoto and capturePhoto Buttons, remove the border
            addPhotoButton.Visibility = Visibility.Hidden;
            capturePhotoButton.Visibility = Visibility.Hidden;
            imageBorder.BorderThickness = new Thickness(0);
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
            if (e.OriginalSource is DataGridCell && e.Key == Key.Delete)
            {
                DeleteSelectedCustomer();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Asks and deletes the selected customer of the datagrid.
        /// Note: Purchases of the customer are automatically set to the anonymous customer (they are not deleted).
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
                ObjectCtx.Context.DeleteObject(currCustomer);
                ObjectCtx.Context.SaveChanges();
            }
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

        public void ExecuteCustomerSale(Customer customer)
        {
            SaleWindow saleWindow = new SaleWindow(customer) { Owner = ParentWindow };
            saleWindow.ShowDialog();
        }
    }
}
