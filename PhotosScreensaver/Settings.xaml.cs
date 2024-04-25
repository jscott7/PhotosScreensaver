using System.Diagnostics;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Forms;

namespace PhotosScreensaver
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            var delay = SettingsUtilities.LoadSetting("delay");
            var path = SettingsUtilities.LoadSetting("photopath");
            var imageDiscoveryMode = SettingsUtilities.LoadSetting("imagediscoverymode");

            if (path != null)
            {
                filePathBox.Text = path.ToString();
            }

            if (delay != null)
            {
                delayTextBox.Text = delay.ToString();
            }
            else
            {
                delayTextBox.Text = "5";
            }
            

            if (imageDiscoveryMode == null)
            {
                DiscoverAllImages.IsChecked = true;
            }
            else
            {
                switch (imageDiscoveryMode.ToString()) 
                {
                    case "FilesInRandomDirectory":
                        ImagesFromRandomDirectory.IsChecked = true;
                        break;
                    case "RandomSelection":
                        RandomSelection.IsChecked = true;
                        break;
                    case "ThisWeekInHistory":
                        ThisWeekInHistory.IsChecked = true;
                        break;
                    default:
                        DiscoverAllImages.IsChecked = true;
                        break;
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(delayTextBox.Text, out int delay))
            {
                SettingsUtilities.SaveSetting("delay", delay);
            }

            SettingsUtilities.SaveSetting("photopath", filePathBox.Text);

            if (DiscoverAllImages.IsChecked.GetValueOrDefault())
            {
                SettingsUtilities.SaveSetting("imagediscoverymode", "AllFiles");
            }
            else if (ImagesFromRandomDirectory.IsChecked.GetValueOrDefault())
            {
                SettingsUtilities.SaveSetting("imagediscoverymode", "FilesInRandomDirectory");
            }
            else if (ThisWeekInHistory.IsChecked.GetValueOrDefault())
                    {
                SettingsUtilities.SaveSetting("imagediscoverymode", "ThisWeekInHistory");
            }
            else if (RandomSelection.IsChecked.GetValueOrDefault())
            {
                SettingsUtilities.SaveSetting("imagediscoverymode", "RandomSelection");
            }
         

            System.Windows.Application.Current.Shutdown();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var fileDialog = new FolderBrowserDialog())
            {
                DialogResult result = fileDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK )
                {
                    filePathBox.Text = fileDialog.SelectedPath;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}