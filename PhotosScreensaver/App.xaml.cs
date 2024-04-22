using System;
using System.Windows;
using System.Globalization;
using System.IO;

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
            var log = new System.Text.StringBuilder();
            try
            {
                log.AppendLine($"Started {DateTime.Now}");

                object rootPath = null;
                if (OperatingSystem.IsWindows())
                {
                    rootPath = SettingsUtilities.LoadSetting("photopath");
                }

                if (rootPath == null)
                {
                    MessageBox.Show("No folder with photos has been set. Open settings to add your folder.", "Photos not found!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                log.AppendLine(rootPath.ToString());

                // Load the image files here so they are available for all screens
                var rootDirectory = new DirectoryInfo(rootPath.ToString());
                var imageDiscoveryMode = FileDiscoveryMode.AllFiles;
                var discoveryModeSetting = "";

                if (OperatingSystem.IsWindows())
                {
                    discoveryModeSetting = SettingsUtilities.LoadSetting("imagediscoverymode")?.ToString();
                }           

                if (!string.IsNullOrEmpty(discoveryModeSetting))
                {
                    imageDiscoveryMode = (FileDiscoveryMode)Enum.Parse(typeof(FileDiscoveryMode), discoveryModeSetting);
                }

                var imageFiles = FileDiscovery.DiscoverImageFiles(rootDirectory, imageDiscoveryMode);

                log.AppendLine($"Loaded {imageFiles.Count} images");
                // If no image files are found with the selected discovery mode, default to loading all files
                if (imageFiles.Count == 0)
                {
                    FileDiscovery.DiscoverImageFiles(rootDirectory, FileDiscoveryMode.AllFiles);            
                }

                int windowIndex = 1;
                // Creates window on each available screen
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
            }
            catch (Exception ex)
            {
                log.Append(ex.ToString());
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            finally
            {
                // Debugging
                // File.WriteAllText(@"C:\Temp\text.txt", log.ToString());
            }
        }
    }
}