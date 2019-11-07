using System;
using System.Windows;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
namespace PhotosScreensaver
{
    /// <summary>
    /// Application level logic. Handling screensaver options and creating the main window(s)
    /// </summary>
    public partial class App : Application
    {
        void OnStartup(Object sender, StartupEventArgs e)
        {
            try
            {
                string[] args = e.Args;
                if (args.Length > 0)
                {
                    // Get the 2 character command line argument
                    string arg = args[0].ToLower(CultureInfo.InvariantCulture).Trim().Substring(0, 2);
                    switch (arg)
                    {
                        case "/c":
                            // Show the options dialog
                            Settings settings = new Settings();
                            settings.Show();
                            break;
                        case "/p":
                            // I'm not putting this in the preview box at the moment
                            ShowScreensaver();
                            break;
                        case "/s":
                            // Show screensaver form
                            ShowScreensaver();
                            break;
                        default:
                            Application.Current.Shutdown();
                            break;
                    }
                }
                else
                {
                    // If no arguments were passed in, show the screensaver
                    ShowScreensaver();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Shows screen saver by creating one instance of PhotoScreenSaver for each monitor.
        /// </summary>
        /// <remarks>
        /// Uses WinForms's Screen class to get monitor info.
        /// </remarks>
        void ShowScreensaver()
        {
            try
            {
                var log = new System.Text.StringBuilder();

                object rootPath = SettingsUtilities.LoadSetting("photopath");
                if (rootPath == null)
                {
                    MessageBox.Show("No folder with photos has been set. Open settings to add your folder.", "Photos not found!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var rootDirectory = new DirectoryInfo(rootPath.ToString());
                List<string> imageFiles = DiscoverImageFiles(rootDirectory);

                int windowIndex = 1;
                // Creates window on each screen
                foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    log.Append($"{screen.DeviceName}:{screen.Bounds}").AppendLine();
                    log.AppendLine("Create Window");
                    var window = new PhotoScreensaver(imageFiles, windowIndex++);

                    window.WindowStartupLocation = WindowStartupLocation.Manual;
                    System.Drawing.Rectangle location = screen.Bounds;

                    //covers entire monitor
                    window.Left = location.X;
                    window.Top = location.Y;
                    window.Width = location.Width;
                    window.Height = location.Height;
                    // Tip, if this isn't the primary window and you set
                    // window.WindowState = WindowState.Maximized;
                    // Before the window has been generated, it'll maximise into the primary

                    // In any case, using normal seems fine.
                    window.WindowState = WindowState.Normal;
                }

                //Show the windows
                foreach (Window window in Current.Windows)
                {
                    log.Append($"Show {window.Width}-{window.Height}-{window.Left}-{window.Top}");
                    window.Show();
                }

                // Debugging
                // System.IO.File.WriteAllText(@"C:\Temp\text.txt", log.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Travers the tree from root directory and save all image filenames
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        /// <remarks>Only .jpg currently supported</remarks>
        private List<string> DiscoverImageFiles(DirectoryInfo directory)
        {
            var imageFiles = new List<string>();

            foreach (var subDirectory in directory.GetDirectories())
            {
                imageFiles.AddRange(DiscoverImageFiles(subDirectory));
            }

            foreach (var imageFile in directory.GetFiles())
            {
                switch (imageFile.Extension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".bmp":
                    case ".png":
                    case ".tiff":
                        imageFiles.Add(imageFile.FullName);
                        break;

                    default:
                        break;
                }
            }

            return imageFiles;
        }
    }
}