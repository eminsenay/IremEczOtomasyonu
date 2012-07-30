using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using IremEczOtomasyonu.BL;
using Microsoft.Expression.Encoder.Devices;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for WebcamWindow.xaml
    /// </summary>
    public partial class WebcamWindow : Window
    {
        private readonly WebcamUtils _webCamUtils;

        public Bitmap GrabbedImage { get; private set; }

        public WebcamWindow()
        {
            InitializeComponent();
            _webCamUtils = new WebcamUtils(host);
            
            // Fill the combobox
            foreach (string encoderDeviceStr in _webCamUtils.GetVideoEncoders())
            {
                webcamComboBox.Items.Add(encoderDeviceStr);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Select the combobox item
            string defaultWebcam = Properties.Settings.Default.SelectedWebcam;
            if (!string.IsNullOrEmpty(defaultWebcam))
            {
                webcamComboBox.SelectedItem = defaultWebcam;
            }
        }

        private void WebcamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Display a preview
            _webCamUtils.DisplayVideoPreview((string) e.AddedItems[0]);
            SaveSelectedWebcam();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _webCamUtils.StopVideoPreview();
        }

        /// <summary>
        /// Saves the selected webcam to use it by default.
        /// </summary>
        private void SaveSelectedWebcam()
        {
            string selectedWebcam = (string) webcamComboBox.SelectedItem;
            if (string.IsNullOrEmpty(selectedWebcam))
            {
                return;
            }
            Properties.Settings.Default.SelectedWebcam = selectedWebcam;
            Properties.Settings.Default.Save();
        }

        private void GrabImageButton_Click(object sender, RoutedEventArgs e)
        {
            GrabbedImage = _webCamUtils.GrabImage();
            WebcamImagePreviewWindow previewWindow = new WebcamImagePreviewWindow(GrabbedImage) { Owner = this };
            bool? dialogResult = previewWindow.ShowDialog();
            if (dialogResult == true)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
