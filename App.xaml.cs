using System;
using System.Windows;
using System.Globalization;

namespace WPFScreenSaver
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

                // Creates window on each screen
                foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    log.Append(screen.DeviceName).Append(":").Append(screen.Bounds).AppendLine();
                    log.AppendLine("Create Window");
                    PhotoScreenSaver window = new PhotoScreenSaver();
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
                foreach (Window window in System.Windows.Application.Current.Windows)
                {
                    log.Append("Show").Append(window.Width).Append("-").Append(window.Height).Append("-").Append(window.Left).Append("-").AppendLine(window.Top.ToString());            
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
    }
}