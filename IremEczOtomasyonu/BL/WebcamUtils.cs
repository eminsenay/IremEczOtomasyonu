using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.Live;

namespace IremEczOtomasyonu.BL
{
    class WebcamUtils
    {
        /// <summary>
        /// Creates job for capture of live source
        /// </summary>
        private LiveJob _job;

        /// <summary>
        /// Device for live source
        /// </summary>
        private LiveDeviceSource _deviceSource;

        /// <summary>
        /// Video preview panel
        /// </summary>
        private readonly Panel _panelVideoPreview;

        /// <summary>
        /// Wpf host interop object
        /// </summary>
        private readonly WindowsFormsHost _host;

        public WebcamUtils(WindowsFormsHost host)
        {
            _host = host;
            _panelVideoPreview = new Panel();
            _host.Child = _panelVideoPreview;
        }

        /// <summary>
        /// Displays a preview from the given video encoder device.
        /// </summary>
        /// <param name="videoDeviceStr">Unique video device string of the encoder.</param>
        public void DisplayVideoPreview(string videoDeviceStr)
        {
            // Find the selected webcam
            var encoderDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
            EncoderDevice videoDevice = encoderDevices.FirstOrDefault(edv => edv.Name == videoDeviceStr);
            if (videoDevice == null)
            {
                return;
            }

            // first stop the current job
            StopVideoPreview();

            // Starts new job for preview window
            _job = new LiveJob();

            // Create a new device source. We use the first audio and video devices on the system
            _deviceSource = _job.AddDeviceSource(videoDevice, null);

            // Webcam configuration dialog is not displayed for now.

            // Setup the video resolution and frame rate of the video device
            // NOTE: Of course, the resolution and frame rate you specify must be supported by the device!
            // NOTE2: May be not all video devices support this call, and so it just doesn't work, 
            // as if you don't call it (no error is raised)
            // NOTE3: As a workaround, if the .PickBestVideoFormat method doesn't work, you could force the resolution 
            // in the following instructions (called few lines belows): 'panelVideoPreview.Size=' and 
            // '_job.OutputFormat.VideoProfile.Size=' to be the one you choosed (640, 480).
            _deviceSource.PickBestVideoFormat(new Size(640, 480), 15);


            // Get the properties of the device video
            SourceProperties sp = _deviceSource.SourcePropertiesSnapshot();

            // Resize the preview panel and the host to match the video device resolution set
            _panelVideoPreview.Size = new Size(sp.Size.Width, sp.Size.Height);
            _host.Width = sp.Size.Width;
            _host.Height = sp.Size.Height;

            // Setup the output video resolution file as the preview
            _job.OutputFormat.VideoProfile.Size = new Size(sp.Size.Width, sp.Size.Height);

            // Sets preview window to winform panel hosted by xaml window
            _deviceSource.PreviewWindow = new PreviewWindow(new HandleRef(
                                                                _panelVideoPreview, _panelVideoPreview.Handle));

            // Make this source the active one
            _job.ActivateSource(_deviceSource);
        }

        /// <summary>
        /// Stops the webcam preview
        /// </summary>
        public void StopVideoPreview()
        {
            if (_job == null)
            {
                return;
            }

            _job.StopEncoding();

            // Remove the Device Source and destroy the job
            _job.RemoveDeviceSource(_deviceSource);

            // Destroy the device source
            _deviceSource.PreviewWindow = null;
            _deviceSource = null;
        }

        public List<string> GetVideoEncoders()
        {
            var encoderDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
            return encoderDevices.Select(edv => edv.Name).ToList();
        }

        /// <summary>
        /// Grabs an image of the active video encoder and returns this image as a bitmap.
        /// </summary>
        /// <returns></returns>
        public Bitmap GrabImage()
        {
            // Create a Bitmap of the same dimension of panelVideoPreview (Width x Height)
            Bitmap bitmap = new Bitmap(_panelVideoPreview.Width, _panelVideoPreview.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Get the paramters to call g.CopyFromScreen and get the image
                Rectangle rectanglePanelVideoPreview = _panelVideoPreview.Bounds;
                Point sourcePoints = _panelVideoPreview.PointToScreen(
                    new Point(_panelVideoPreview.ClientRectangle.X, _panelVideoPreview.ClientRectangle.Y));
                g.CopyFromScreen(sourcePoints, Point.Empty, rectanglePanelVideoPreview.Size);
            }

            return bitmap;
        }
    }
}
