using System;

namespace PhotosScreensaver
{
    /// <summary>
    /// Interface for controlling display of image labels
    /// </summary>
    public interface ILabelDisplay
    {
        /// <summary>
        /// Return a label based on a URI
        /// </summary>
        /// <param name="uri">The full uri for the image file</param>
        /// <returns>A human readable label</returns>
        string ImageSrcFromFileUri(Uri uri);
    }
}
