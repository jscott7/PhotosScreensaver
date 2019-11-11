using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosScreensaver
{
    public interface ILabelDisplay
    {
        string ImageSrcFromFileUri(Uri uri);
    }
}
