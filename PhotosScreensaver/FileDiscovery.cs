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
                case FileDiscoveryMode.ThisWeekInHistory:
                    return DiscoverImageFilesForThisWeek(directory);
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

            GetFilesForSingleDirectory(directory, imageFiles, false);

            return imageFiles;
        }

        private static List<string> DiscoverImageFilesForThisWeek(DirectoryInfo directory)
        {
            var imageFiles = new List<string>();

            foreach (var subDirectory in directory.GetDirectories())
            {
                imageFiles.AddRange(DiscoverImageFilesForThisWeek(subDirectory));
            }

            GetFilesForSingleDirectory(directory, imageFiles, true);

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
            GetFilesForSingleDirectory(directoryToUse, imageFiles, false);
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

        /// <summary>
        /// Adds files matching image type to list
        /// </summary>
        /// <param name="directory">Directory containing files</param>
        /// <param name="imageFiles">List of valid filanames</param>
        /// <param name="thisWeek">Flag indicating whether to keep only files that were created within +- 7 days of current date</param>
        private static void GetFilesForSingleDirectory(DirectoryInfo directory, List<string> imageFiles, bool thisWeek)
        {     
            foreach (var imageFile in directory.GetFiles())
            {
                if (thisWeek && !IsCreatedInTimeLimit(imageFile, DateTime.Now))
                {
                    continue;
                }

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

        internal static bool IsCreatedInTimeLimit(FileInfo imageFile, DateTime referenceDate)
        {
            var upper = referenceDate.AddDays(7);
            var lower = referenceDate.AddDays(-7);
            var created = imageFile.CreationTime;
            return created.Month >= lower.Month &&
                   created.Month <= upper.Month &&
                   created.Day >= lower.Day &&
                   created.Day <= upper.Day;
        }
    }
}
