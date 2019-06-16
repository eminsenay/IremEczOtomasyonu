using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using IremEczOtomasyonu.Models;
using IremEczOtomasyonu.BL;
using Microsoft.EntityFrameworkCore;

namespace IremEczOtomasyonu.UI
{
    /// <summary>
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {
        private readonly ICollectionView _customerView;
        public Customer SelectedCustomer { get; set; }

        public CustomerListWindow()
        {
            InitializeComponent();

            CollectionViewSource customersViewSource = ((CollectionViewSource)(FindResource("CustomersViewSource")));
            customersViewSource.Source = ObjectCtx.Context.Customers;
            _customerView = customersViewSource.View;
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

        private void CustomersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateAndClose();
        }

        private void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateAndClose();
        }

        private void UpdateAndClose()
        {
            SelectedCustomer = customersDataGrid.SelectedItem as Customer;
            DialogResult = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedCustomer == null)
            {
                return;
            }
            customersDataGrid.SelectedItem = SelectedCustomer;
        }
    }
}
