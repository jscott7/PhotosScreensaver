using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;

namespace WPFScreenSaver
{
    /// <summary>
    /// Interaction logic for PhotoScreenSaver
    /// </summary>
    public partial class PhotoScreenSaver : Window, IDisposable, INotifyPropertyChanged
    {
        private List<string> ImageFiles = new List<string>();
        private Random RandomGenerator;
        private bool IsMouseActive;
        private Point MousePosition;
        private bool Disposed;
        private string CurrentImageSrc = string.Empty;

        // The Timer must be a private object otherwise it will get garbage collected and stop
        private Timer UpdateTimer;

        public event PropertyChangedEventHandler PropertyChanged;
        public string ImageSrc
        {
            get 
            {
                return this.CurrentImageSrc; 
            }
            set
            {
                this.CurrentImageSrc = value;
                OnPropertyChanged("ImageSrc");
            }
        }

        public PhotoScreenSaver(List<string> imageFiles, int windowIndex)
        {
            // This is required to get the binding hooked up to the XAML
            DataContext = this;

            InitializeComponent();

            ImageFiles = imageFiles;

            RandomGenerator = new Random(windowIndex * (DateTime.Now.Hour * 1000000 + DateTime.Now.Minute * 10000 + DateTime.Now.Millisecond));
          
            var screenHeight = this.ActualHeight;
            var screenWidth = this.ActualWidth;
            this.Height = screenHeight;
            this.Width = screenWidth;

            // Setup Timer for updating images
            // Timeperiod needs to be configured by settings
            TimerCallback callback = ShowNextImage;
            object delay = SettingsUtilities.LoadSetting("delay");
            int timerPeriod = 5000;
            if (int.TryParse(delay.ToString(), out int settingsPeriod))
            {
                timerPeriod = settingsPeriod * 1000;
            }

            UpdateTimer = new Timer(callback, null, 0, timerPeriod);
        }

        void OnLoaded(object sender, EventArgs e)
        {

#if !DEBUG
            Topmost = true;
            MouseMove += new MouseEventHandler(PhotoStack_MouseMove);
            MouseDown += new MouseButtonEventHandler(PhotoStack_MouseDown);
            KeyDown += new KeyEventHandler(PhotoStack_KeyDown);
#endif
        }

        void PhotoStack_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void PhotoStack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void PhotoStack_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = e.MouseDevice.GetPosition(this);
            // Set IsActive and MouseLocation only the first time this event is called.
            if (!IsMouseActive)
            {
                MousePosition = currentPosition;
                IsMouseActive = true;
            }
            else
            {
                // If the mouse has moved significantly since first call, close.
                if ((Math.Abs(MousePosition.X - currentPosition.X) > 10) ||
                    (Math.Abs(MousePosition.Y - currentPosition.Y) > 10))
                {
                    Application.Current.Shutdown();
                }
            }
        }

        void ShowNextImage(Object stateInfo)
        {
            // Need InvokeRequired equivalent here...
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                // Use Action so we don't have to explictly define a delegate
                this.Dispatcher.Invoke(new Action<Object>(ShowNextImage), new object[] { stateInfo });
            }
            else
            {
                // We need the current screen dimensions
                // Actual height and width isn't necessarily the same as PrimaryScreenHeight and width
                // In my case they were both 16 pixels greater. Since the window has been defined as maximised in xaml we can used these
                // to resize the images
                // double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                // double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                var screenHeight = this.ActualHeight;
                var screenWidth = this.ActualWidth;

                var imageFileCount = ImageFiles.Count;
                var index = RandomGenerator.NextDouble() * imageFileCount;

                // Open a Uri and decode the image
                var filename = ImageFiles[(int)index];
        
                Uri myUri = new Uri(filename, UriKind.RelativeOrAbsolute);
                ImageSrc = ImageSrcFromFileUri(myUri);  
                var image = System.Drawing.Image.FromFile(filename);
   
                int rotationIndex = GetRotationIndex(image);    

                try
                {
                    var bitmapImage = ConvertImageToBitmapImage(image, rotationIndex);

                    // Draw the Image
                    ScreenImage.Source = bitmapImage;

                    // Resize the image
                    // If the height is the largest dimension, set height to screen height and scale width
                    // And vice versa
                    var imgHeight = image.Height;
                    var imgWidth = image.Width;
                    if (imgHeight >= imgWidth || rotationIndex > 1)
                    {
                        // This is portrait. Recalculate the width of the columns to centralise the image
                        var stretchRatio = screenHeight / imgHeight;
                        ScreenImage.Width = imgWidth * stretchRatio;
                        ScreenImage.Height = screenHeight;

                        Col1.Width = new GridLength((screenWidth - ScreenImage.Width) / 2);
                        Col2.Width = new GridLength((screenWidth - ScreenImage.Width) / 2);
                    }
                    else
                    {
                        // This is landscape. Stretch to fill the screen
                        var stretchRatio = screenWidth / imgWidth;
                        ScreenImage.Width = screenWidth;
                        ScreenImage.Height = imgHeight * stretchRatio;
                        Col1.Width = new GridLength(0);
                        Col2.Width = new GridLength(0);
                    }
                }
                catch (Exception)
                {
                    // Suppress exceptions unless we want to explicitly debug here
                    //if (ex.InnerException != null)
                    //{
                    //    LogFile.WriteLine("{0} for {1}", ex.InnerException.Message, filename);
                    //}
                    //else
                    //{
                    //    LogFile.WriteLine("{0} for {1}", ex.Message, filename);
                    //}
                    ShowNextImage(stateInfo);
                    return;
                }
            }
        }

        private string ImageSrcFromFileUri(Uri uri)
        {
            string[] fileComponents = uri.AbsolutePath.Split('/');
            int length = fileComponents.Length;
            if (length > 3)
            {
                return string.Join("/", fileComponents[length - 3], fileComponents[length - 2]);
            }
            else
            {
                return uri.AbsolutePath;
            }
        }

        /// <summary>
        /// Inspect the EXIF metadata for orientation (0x112)
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private int GetRotationIndex(System.Drawing.Image image)
        {
            int rotationIndex = 0;
            try
            {
                var prop = image.GetPropertyItem(0x112);

                int val = BitConverter.ToUInt16(prop.Value, 0);
                if (val > 0)
                {
                    rotationIndex = val;
                }
            }
            catch (ArgumentException) { }

            return rotationIndex;
        }

        /// <summary>
        /// Get the image format enum from filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The ImageFormat for the filename extension, or default ImageFormat.Jpeg</returns>
        /// <remarks>
        /// This is a naieve check on filename extension
        /// </remarks>
        private static System.Drawing.Imaging.ImageFormat GetImageFormat(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            switch (fileInfo.Extension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
                case ".gif":
                    return System.Drawing.Imaging.ImageFormat.Gif;
                case ".bmp":
                    return System.Drawing.Imaging.ImageFormat.Bmp;
                case ".png":
                    return System.Drawing.Imaging.ImageFormat.Png;
                case ".tiff":
                    return System.Drawing.Imaging.ImageFormat.Tiff;
                default:
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
            }
        }

        private static BitmapImage ConvertImageToBitmapImage(System.Drawing.Image image, int rotationIndex)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                if (rotationIndex == 6)
                {
                    bitmapImage.Rotation = Rotation.Rotate90;
                }
                if (rotationIndex == 8)
                {
                    bitmapImage.Rotation = Rotation.Rotate270;
                }

                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                UpdateTimer.Dispose();
            }

            Disposed = true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}