using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace IremEczOtomasyonu.UI
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
            BindingExpression expression = barcodeTextBox.GetBindingExpression(TextBox.TextProperty);
            if (expression != null)
            {
                expression.UpdateSource();
            }
            if (Validation.GetHasError(barcodeTextBox))
            {
                MessageBox.Show(Validation.GetErrors(barcodeTextBox)[0].ErrorContent.ToString());
                e.Handled = true;
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Close the window if the Esc key is pressed
            if (e.Key != Key.Escape)
            {
                return;
            }
            DialogResult = false;
            e.Handled = true;
            Close();
        }
    }
}
