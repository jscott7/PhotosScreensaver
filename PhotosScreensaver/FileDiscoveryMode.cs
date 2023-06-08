namespace PhotosScreensaver
{
    /// <summary>
    /// The supported modes for discovery of image files
    /// </summary>
    enum FileDiscoveryMode
    {
        /// <summary>
        /// Use all available image files in directory tree
        /// </summary>
        AllFiles,

        /// <summary>
        /// Use image files in a random directory containing more than 100 files
        /// </summary>
        FilesInRandomDirectory,

        /// <summary>
        /// Use a random choice of AllFiles or FilesInRandomDirectory
        /// </summary>
        RandomSelection
    }
}
