using System;
using System.Collections.Generic;
using System.IO;

namespace PhotosScreensaver
{
    class FileDiscovery
    {
        /// <summary>
        /// Discover image files 
        /// </summary>
        /// <param name="directory">The root directory for starting the discovery</param>
        /// <param name="fileDiscoveryMode"></param>
        /// <returns>Full path to all discovered images</returns>
        public static List<string> DiscoverImageFiles(DirectoryInfo directory, FileDiscoveryMode fileDiscoveryMode)
        {
            switch(fileDiscoveryMode)
            {
                case FileDiscoveryMode.AllFiles:
                    return DiscoverImageFilesForAllDirectories(directory);
                case FileDiscoveryMode.FilesInRandomDirectory:
                    return DiscoverImageFilesForRandomDirectory(directory);
                case FileDiscoveryMode.RandomSelection:
                    var random = new Random();
                    var randomDiscoveryMode = (FileDiscoveryMode)random.Next(2);
                    return DiscoverImageFiles(directory, randomDiscoveryMode);
                default:
                    return DiscoverImageFilesForAllDirectories(directory);
            }
        }

        /// <summary>
        /// Traverse the tree from root directory and save all image filenames
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static List<string> DiscoverImageFilesForAllDirectories(DirectoryInfo directory)
        {
            var imageFiles = new List<string>();

            foreach (var subDirectory in directory.GetDirectories())
            {
                imageFiles.AddRange(DiscoverImageFilesForAllDirectories(subDirectory));
            }

            GetFilesForSingleDirectory(directory, imageFiles);

            return imageFiles;
        }

        /// <summary>
        /// Select a random directory in tree with more than a minimum number of files and return all images from that directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static List<string> DiscoverImageFilesForRandomDirectory(DirectoryInfo directory)
        {
            var candidateDirectories = DiscoverDirectoriesAboveMinSize(directory, 100);
            var random = new Random(DateTime.Now.Hour * 1000000 + DateTime.Now.Minute * 10000 + DateTime.Now.Millisecond);
            var index = random.NextDouble() * candidateDirectories.Count;

            // Open a Uri and decode the image
            var directoryToUse = candidateDirectories[(int)index];

            var imageFiles = new List<string>();
            GetFilesForSingleDirectory(directoryToUse, imageFiles);
            return imageFiles;
        }

        /// <summary>
        /// Discover directories in tree with more than a given number of files
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="minSize">Include Directory if it contains >= minSize number of files</param>
        /// <returns></returns>
        private static List<DirectoryInfo> DiscoverDirectoriesAboveMinSize(DirectoryInfo directory, int minSize)
        {
            var directories = new List<DirectoryInfo>();
            foreach (var subDirectory in directory.GetDirectories())
            {
                directories.AddRange(DiscoverDirectoriesAboveMinSize(subDirectory, minSize));
            }
   
            if (directory.GetFiles().Length > minSize)
            {
                directories.Add(directory);
            }

            return directories;      
        }

        private static void GetFilesForSingleDirectory(DirectoryInfo directory, List<string> imageFiles)
        {     
            foreach (var imageFile in directory.GetFiles())
            {
                switch (imageFile.Extension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".bmp":
                    case ".png":
                    case ".tiff":
                        imageFiles.Add(imageFile.FullName);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
