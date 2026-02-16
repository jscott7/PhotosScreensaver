using System;
using NUnit.Framework;

namespace PhotosScreensaver.Tests
{
    [TestFixture]
    public class DisplayPathFromYearTests
    {
        [Test]
        public void ReturnsPathFromYear_WhenYearPresent()
        {
            var displayPathFromYear = new DisplayPathFromYear();
            var uri = new Uri("file:///C:/Temp/Photos/2019/November/London/img_01020.jpg");

            var result = displayPathFromYear.ImageSrcFromFileUri(uri);

            Assert.That(result, Is.EqualTo("2019/November/London"));
        }

        [Test]
        public void ReturnsOriginalPath_WhenNoYearPresent()
        {
            var displayPathFromYear = new DisplayPathFromYear();
            var uri = new Uri("file:///C:/Temp/Photos/NoYear/London/img.jpg");

            var result = displayPathFromYear.ImageSrcFromFileUri(uri);

            Assert.That(result, Is.EqualTo(Uri.UnescapeDataString(uri.AbsolutePath)));
        }

        [Test]
        public void UnescapesPercentEncodedSegments_WhenYearPresent()
        {
            var displayPathFromYear = new DisplayPathFromYear();
            var uri = new Uri("file:///C:/Photos/2018/My%20Photos/img.jpg");

            var result = displayPathFromYear.ImageSrcFromFileUri(uri);

            Assert.That(result, Is.EqualTo("2018/My Photos"));
        }

        [Test]
        public void MatchesFirstFourDigitSegment_WhenMultipleYearsExist()
        {
            var displayPathFromYear = new DisplayPathFromYear();
            var uri = new Uri("file:///C:/2020/Photos/2021/July/img.jpg");

            var result = displayPathFromYear.ImageSrcFromFileUri(uri);

            Assert.That(result, Is.EqualTo("2020/Photos/2021/July"));
        }

        [Test]
        public void DoesNotMatchNonFourDigitNumericSegments()
        {
            var displayPathFromYear = new DisplayPathFromYear();
            var uri = new Uri("file:///C:/Temp/12345/April/img.jpg");

            var result = displayPathFromYear.ImageSrcFromFileUri(uri);

            Assert.That(result, Is.EqualTo(Uri.UnescapeDataString(uri.AbsolutePath)));
        }
    }
}