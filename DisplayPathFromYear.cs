using System;
using System.Text;
using System.Text.RegularExpressions;

namespace PhotosScreensaver
{
    /// <summary>
    /// Given a URI that contains a year, print the year to final folder
    /// e.g.
    /// file://C:/Temp/Photos/2019/November/London/img_01020.jpg
    /// returns
    /// 2019/November/London
    /// </summary>
    public class DisplayPathFromYear : ILabelDisplay
    {
        public string ImageSrcFromFileUri(Uri uri)
        {
            var path = Uri.UnescapeDataString(uri.AbsolutePath);
            var pathComponents = path.Split('/');
            var regexPattern = @"^\d{4}$";
            int length = pathComponents.Length;
            int startIndex = -1;

            for (int pathIndex = 0; pathIndex < length - 1; pathIndex++)
            {
                Match m = Regex.Match(pathComponents[pathIndex], regexPattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    startIndex = pathIndex;
                    break;
                }
            }

            if (startIndex != -1)
            {
                var stringBuilder = new StringBuilder();
                for (int index = startIndex; index < length - 1; index++)
                {
                    stringBuilder.Append(pathComponents[index]).Append("/");
                }

                // Trim the trailing '/'
                return stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
            }
            else
            {
                return path;
            }
        }
    }
}
