using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WPFScreenSaver
{
    /// <summary>
    /// Interaction logic for PhotoScreenSaver
    /// </summary>
    public partial class PhotoScreenSaver : Window
    {
        private List<string> ImageFiles = new List<string>();
        private Random RandomGenerator;
        private bool IsMouseActive;
        private Point MousePosition;

        // The Timer must be a private object otherwise it will get garbage collected and stop
        private Timer UpdateTimer;
 
        public PhotoScreenSaver()
        {
            InitializeComponent();

            // This needs to be configured via settings
            object rootPath = SettingsUtilities.LoadSetting("photopath");
            if ( rootPath == null )
            {
                MessageBox.Show("No folder with photos has been set. Open settings to add your folder.", "Photos not found!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var baseDirectory = new DirectoryInfo(rootPath.ToString());
            FileDiscovery(baseDirectory);

            RandomGenerator = new Random(DateTime.Now.Hour * 1000000 + DateTime.Now.Minute * 10000 + DateTime.Now.Millisecond);
          
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

        public void ShowNextImage(Object stateInfo)
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
                BitmapImage image = new BitmapImage();

                try
                {
                    image.BeginInit();
                    image.UriSource = myUri;
                    image.EndInit();
                }
                catch(Exception ex)
                {
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

                // Draw the Image
                ScreenImage.Source = image;

                // Resize the image
                // If the height is the largest dimension, set height to screen height and scale width
                // And vice versa
                var imgHeight = image.Height;
                var imgWidth = image.Width;
                if (imgHeight >= imgWidth)
                {
                    var stretchRatio = screenHeight / imgHeight;
                    ScreenImage.Width = imgWidth * stretchRatio;
                    ScreenImage.Height = screenHeight;

                    Col1.Width = new GridLength((screenWidth - ScreenImage.Width) / 2);
                    Col2.Width = new GridLength((screenWidth - ScreenImage.Width) / 2);
                }
                else
                {
                    var stretchRatio = screenWidth / imgWidth;
                    ScreenImage.Width = screenWidth;
                    ScreenImage.Height = imgHeight * stretchRatio;
                    Col1.Width = new GridLength(0);
                    Col2.Width = new GridLength(0);
                }
            }
        }

        private void FileDiscovery(DirectoryInfo directory)
        {
            foreach (var subDirectory in directory.GetDirectories())
            {
                FileDiscovery(subDirectory);
                foreach (var imageFile in subDirectory.GetFiles())
                {
                    if (imageFile.Extension.ToLower() == ".jpg")
                    {
                        ImageFiles.Add(imageFile.FullName);
                    }
                }               
            }
        }
    }
}