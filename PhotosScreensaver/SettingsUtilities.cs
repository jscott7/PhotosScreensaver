using Microsoft.Win32;
using System.Runtime.Versioning;

namespace PhotosScreensaver
{
    public class SettingsUtilities
    {
        private static readonly string _keyName = "SOFTWARE\\JscoPhotoScreenSaver";

        /// <summary>
        /// Saves a setting to the Registry
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [SupportedOSPlatform("windows")]
        public static void SaveSetting(string name, object value)
        {
            var key = Registry.CurrentUser.CreateSubKey(_keyName);

            if (value != null)
            {
                key.SetValue(name, value);
            }
        }

        /// <summary>
        /// Loads a setting from the Registry
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public static object LoadSetting(string name)
        {
            // Get the value stored in the Registry  
            var key = Registry.CurrentUser.OpenSubKey(_keyName);

            if (key == null)
            {
                return null;
            }
            else
            {
                return key.GetValue(name);
            }
        }
    }
}
