using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using OpenCvSharp;
using System.Threading;
using OpenCvSharp.Extensions;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using WebCamUtils;

namespace IremEczOtomasyonu.UI
{
    /// <summary>
    /// Interaction logic for WebcamWindow.xaml
    /// </summary>
    public partial class WebcamWindow
    {
        public Bitmap GrabbedImage { get; private set; }
        private VideoCapture _videoCapture;
        private Thread _cameraThread;
        private bool _isCameraRunning = true;
        private int _selectedWebCamIndex = 0;


        public WebcamWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Refresh the list of available cameras
            webcamComboBox.Items.Clear();
            string defaultWebcam = Properties.Settings.Default.SelectedWebcam;

            foreach (FilterInfo cam in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                webcamComboBox.Items.Add(cam);
                // Search for the last selected webcam
                if (cam.Name == defaultWebcam)
                {
                    webcamComboBox.SelectedItem = cam;
                }
            }
            _selectedWebCamIndex = webcamComboBox.SelectedIndex;
        }

        private void WebcamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveSelectedWebcam();
            // Display a preview
            StopCapturing();
            StartCapturing();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopCapturing();
        }

        /// <summary>
        /// Saves the selected webcam to use it by default.
        /// </summary>
        private void SaveSelectedWebcam()
        {
            string selectedWebcam = ((FilterInfo)webcamComboBox.SelectedItem).Name;
            if (string.IsNullOrEmpty(selectedWebcam))
            {
                return;
            }
            Properties.Settings.Default.SelectedWebcam = selectedWebcam;
            Properties.Settings.Default.Save();
            _selectedWebCamIndex = webcamComboBox.SelectedIndex;
        }

        private void GrabImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isCameraRunning)
            {
                return;
            }
            // Clone the image to use it in preview window
            GrabbedImage = ConvertBitmapSource2Bitmap((BitmapSource)pictureBox.Source);

            WebcamImagePreviewWindow previewWindow = new WebcamImagePreviewWindow(GrabbedImage) { Owner = this };
            bool? dialogResult = previewWindow.ShowDialog();
            // If the user accepts to use the image, close the window
            if (dialogResult == true)
            {
                DialogResult = true;
                Close();
            }
        }

        /// <summary>
        /// Start capturing from the selected webcam
        /// </summary>
        private void StartCapturing()
        {
            _cameraThread = new Thread(new ThreadStart(CaptureCameraCallback));
            _isCameraRunning = true;
            _cameraThread.Start();
        }

        private void CaptureCameraCallback()
        {
            Mat frame = new Mat();
            _videoCapture = new VideoCapture();
            _videoCapture.Open(_selectedWebCamIndex); 

            if (!_videoCapture.IsOpened())
            {
                return;
            }

            while (_isCameraRunning)
            {
                try
                {
                    _videoCapture.Read(frame);
                    Bitmap image = BitmapConverter.ToBitmap(frame);
                    Dispatcher.Invoke(() => pictureBox.Source = ConvertBitmap2BitmapImage(image));
                    image.Dispose();
                }
                catch (ArgumentException)
                {
                    // During webcam switches for a short period of time, the frame becomes an invalid argument.
                    // Do nothing, it heals itsel.
                }
            }
        }

        private BitmapImage ConvertBitmap2BitmapImage(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static Bitmap ConvertBitmapSource2Bitmap(BitmapSource bitmapSource)
        {
            var width = bitmapSource.PixelWidth;
            var height = bitmapSource.PixelHeight;
            var stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            var memoryBlockPointer = Marshal.AllocHGlobal(height * stride);
            bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), memoryBlockPointer, height * stride, stride);
            var bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppPArgb, memoryBlockPointer);
            return bitmap;
        }

        private void StopCapturing()
        {
            lock(this)
            {
                _isCameraRunning = false;
            }
        }

    }
}
