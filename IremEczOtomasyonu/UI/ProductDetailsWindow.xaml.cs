﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using IremEczOtomasyonu.Models;
using IremEczOtomasyonu.BL;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;

namespace IremEczOtomasyonu.UI
{
    /// <summary>
    /// Interaction logic for ProductDetailsWindow.xaml
    /// </summary>
    public partial class ProductDetailsWindow : Window
    {
        private readonly Product _product;
        private readonly ObservableCollection<ExpirationDate> _expirationDates;

        public ProductDetailsWindow(Product product)
        {
            InitializeComponent();
            _product = product;
            _expirationDates = new ObservableCollection<ExpirationDate>(_product.ExpirationDates);
            _expirationDates.CollectionChanged += ExpirationDates_CollectionChanged;

            productDetailsGrid.DataContext = _product;

            CollectionViewSource expirationDatesViewSource = ((CollectionViewSource)(FindResource("ExpirationDatesViewSource")));
            expirationDatesViewSource.Source = _expirationDates;

            dealsDataGrid.ItemsSource = _product.GetAllDeals();
        }

        private void ExpirationDates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ExpirationDate expirationDate in e.NewItems)
                {
                    expirationDate.Id = Guid.NewGuid();
                    _product.ExpirationDates.Add(expirationDate);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ExpirationDate expirationDate in e.OldItems)
                {
                    _product.ExpirationDates.Remove(expirationDate);
                    ObjectCtx.Context.ExpirationDates.Remove(expirationDate);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(barcodeTextBox) || Validation.GetHasError(nameTextBox) ||
                Validation.GetHasError(brandTextBox) || Validation.GetHasError(currentBuyingPriceTextBox) ||
                Validation.GetHasError(currentSellingPriceTextBox) || 
                UIUtilities.HasDataGridErrors(expirationDatesDataGrid))
            {
                MessageBox.Show("Girdiğiniz bazı bilgiler eksik ya da hatalı. \n Lütfen düzeltip tekrar deneyin.",
                                "Ürün değiştirme uyarısı", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string detailedValidationMessage = _product.Validate();
            if (!string.IsNullOrEmpty(detailedValidationMessage))
            {
                MessageBox.Show(detailedValidationMessage, "Ürün değiştirme uyarısı", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            ObjectCtx.Context.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void DatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            UIUtilities.DatePickerSelectDecade(sender as DatePicker);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == true)
            {
                return;
            }
            
            // Remove possibly added expiration dates
            foreach (var entry in ObjectCtx.Context.ChangeTracker.Entries().Where(
                t => t.State == Microsoft.EntityFrameworkCore.EntityState.Added).ToList())
            {
                if (entry.Entity != null)
                {
                    ObjectCtx.Context.Remove(entry.Entity);
                }
            }
            ObjectCtx.Context.SaveChanges();
        }

        private void DatagridDeleteExpirationDateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExpirationDate expirationDate = expirationDatesDataGrid.SelectedItem as ExpirationDate;
            if (expirationDate == null)
            {
                return;
            }

            _expirationDates.Remove(expirationDate);
        }
    }
}
