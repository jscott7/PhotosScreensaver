using NUnit.Framework;
using PhotosScreensaver;

namespace PhotosScreensaverTests
{
    public class TestLabelDisplay
    {

        [TestCase("file://C:/Temp/Photos/2019/November/London/img_01020.jpg", "2019/November/London")]
        [TestCase("file://C:/Temp/Photos/2023/img_01020.jpg", "2023")]
        [TestCase("file://C:/Temp/Photos/NoYear/img_01020.jpg", "C:/Temp/Photos/NoYear/img_01020.jpg")]
        public void Display_Correct_Path_From_Year(string uri, string expectedYear)
        {
            var displayPathFromYear = new DisplayPathFromYear();
            var year = displayPathFromYear.ImageSrcFromFileUri(new Uri(uri));
            Assert.That(year, Is.EqualTo(expectedYear));
        }
    }
}