using System.Windows;
using System.Windows.Forms;

namespace WPFScreenSaver
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
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(delayTextBox.Text, out int delay))
            {
                SettingsUtilities.SaveSetting("delay", delay);
            }

            SettingsUtilities.SaveSetting("photopath", filePathBox.Text);
            System.Windows.Application.Current.Shutdown();
        }

        private void browse_Click(object sender, RoutedEventArgs e)
        {
            using (var fileDialog = new FolderBrowserDialog())
            {
                DialogResult result = fileDialog.ShowDialog();
                if ( result == System.Windows.Forms.DialogResult.OK )
                {
                    filePathBox.Text = fileDialog.SelectedPath;
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}