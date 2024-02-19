using NUnit.Framework;
using PhotosScreensaver;
namespace PhotosScreensaverTests
{
    public class TestFileDiscovery
    {
        [TestCase("2024-01-20", true)]
        [TestCase("2022-01-20", true)]
        [TestCase("2024-01-16", true)]
        [TestCase("2024-01-15", false)]
        [TestCase("2024-02-20", false)]
        [TestCase("2019-02-20", false)]
        public void FileCreationTime_Is_InLimit(string createdTime, bool expected)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var testFile = new FileInfo(assembly.Location);
            testFile.CreationTime = DateTime.Parse(createdTime);
            var referenceDate = DateTime.Parse("2024-01-23");
            var actual = FileDiscovery.IsCreatedInTimeLimit(testFile, referenceDate);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
