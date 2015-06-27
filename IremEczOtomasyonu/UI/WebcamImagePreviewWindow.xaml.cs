using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace IremEczOtomasyonu.UI
{
    /// <summary>
    /// Interaction logic for WebcamImagePreviewWindow.xaml
    /// </summary>
    public partial class WebcamImagePreviewWindow : Window
    {
        public WebcamImagePreviewWindow(Bitmap image)
        {
            InitializeComponent();

            using (MemoryStream memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                webcamImage.Source = bitmapImage;
            }
        }

        private void UseImageButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void GrabAnotherImageButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
