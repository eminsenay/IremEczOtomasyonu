using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Touchless.Vision.Camera;
using MessageBox = System.Windows.MessageBox;

namespace IremEczOtomasyonu.UI
{
    /// <summary>
    /// Interaction logic for WebcamWindow.xaml
    /// </summary>
    public partial class WebcamWindow : Window
    {
        public Bitmap GrabbedImage { get; private set; }
        private static Bitmap _latestFrame;
        private CameraFrameSource _frameSource;
        private readonly PictureBox _pictureBoxDisplay;

        public WebcamWindow()
        {
            InitializeComponent();
            _pictureBoxDisplay = new PictureBox();
            host.Child = _pictureBoxDisplay;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Refresh the list of available cameras
            webcamComboBox.Items.Clear();
            string defaultWebcam = Properties.Settings.Default.SelectedWebcam;
            foreach (Camera cam in CameraService.AvailableCameras)
            {
                webcamComboBox.Items.Add(cam);
                // Search for the last selected webcam
                if (cam.Name == defaultWebcam)
                {
                    webcamComboBox.SelectedItem = cam;
                }
            }
        }

        private void WebcamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Display a preview
            ThrashOldCamera();
            StartCapturing();
            SaveSelectedWebcam();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ThrashOldCamera();
        }

        /// <summary>
        /// Saves the selected webcam to use it by default.
        /// </summary>
        private void SaveSelectedWebcam()
        {
            string selectedWebcam = ((Camera)webcamComboBox.SelectedItem).Name;
            if (string.IsNullOrEmpty(selectedWebcam))
            {
                return;
            }
            Properties.Settings.Default.SelectedWebcam = selectedWebcam;
            Properties.Settings.Default.Save();
        }

        private void GrabImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_frameSource == null)
            {
                return;
            }

            // Clone the image to use it in preview window
            GrabbedImage = (Bitmap) _latestFrame.Clone();
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
            try
            {
                Camera c = (Camera)webcamComboBox.SelectedItem;
                SetFrameSource(new CameraFrameSource(c));
                _frameSource.Camera.CaptureWidth = 320;
                _frameSource.Camera.CaptureHeight = 240;
                _frameSource.Camera.Fps = 20;
                _frameSource.NewFrame += OnImageCaptured;

                _pictureBoxDisplay.Paint += DrawLatestImage;
                _frameSource.StartFrameCapture();
            }
            catch (Exception ex)
            {
                webcamComboBox.Text = "Bir Webcam Seçiniz";
                MessageBox.Show(ex.Message);
            }
        }

        public void OnImageCaptured(Touchless.Vision.Contracts.IFrameSource frameSource, 
            Touchless.Vision.Contracts.Frame frame, double fps)
        {
            _latestFrame = frame.Image;
            _pictureBoxDisplay.Invalidate();
        }

        private void DrawLatestImage(object sender, PaintEventArgs e)
        {
            if (_latestFrame != null)
            {
                // Draw the latest image from the active camera
                e.Graphics.DrawImage(_latestFrame, 0, 0, _latestFrame.Width, _latestFrame.Height);
            }
        }

        private void ThrashOldCamera()
        {
            // Trash the old camera
            if (_frameSource != null)
            {
                _frameSource.NewFrame -= OnImageCaptured;
                _frameSource.Camera.Dispose();
                SetFrameSource(null);
                _pictureBoxDisplay.Paint -= DrawLatestImage;
            }
        }

        private void SetFrameSource(CameraFrameSource cameraFrameSource)
        {
            if (_frameSource == cameraFrameSource)
            {
                return;
            }

            _frameSource = cameraFrameSource;
        }
    }
}
