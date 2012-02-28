﻿using System;
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
using System.Windows.Shapes;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for BarcodeWindow.xaml
    /// </summary>
    public partial class BarcodeWindow : Window
    {
        public string Barcode
        {
            get { return (barcodeTextBox == null || string.IsNullOrEmpty(barcodeTextBox.Text)) ? null : 
                barcodeTextBox.Text; }
        }
        /// <summary>
        /// Initializes a new barcode window.
        /// </summary>
        public BarcodeWindow()
        {
            InitializeComponent();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
