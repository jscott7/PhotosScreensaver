using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotosScreensaver
{
    /// <summary>
    /// Given a URI that contains a year, print the year to final folder
    /// e.g.
    /// file://C:/Temp/Photos/2019/November/London/img_01020.jpg
    /// returns
    /// 2019/November/London
    /// </summary>
    public class UrlFromYearToString : IUrlPrettyPrint
    {
        public string ImageSrcFromFileUri(Uri uri)
        {
            string path = Uri.UnescapeDataString(uri.AbsolutePath);
            string[] pathComponents = path.Split('/');

            string regexPattern = @"^\d{4}$";
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
                return string.Join("/", pathComponents[startIndex], pathComponents[length - 2]);
            }
            else
            {
                return path;
            }
        }
    }
}
