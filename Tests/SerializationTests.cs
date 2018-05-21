using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using param.sfo.editor;

namespace Tests
{
    [TestFixture]
    public class SerializationTests
    {
        private string[] GetTestFiles()
        {
            var path = TestContext.CurrentContext.TestDirectory;
            path = Path.Combine(path, @"..\..\TestFiles");
            if (!Directory.Exists(path))
                Assert.Fail("Directory with test files couldn't be found");

            return Directory.GetFiles(path, "*.sfo", SearchOption.TopDirectoryOnly);
        }

        [Test]
        public void ReadingTest()
        {
            var testFiles = GetTestFiles();
            foreach (var testFile in testFiles)
            {
                var filename = Path.GetFileNameWithoutExtension(testFile);
                TestContext.Progress.WriteLine(filename);
                using (var originalStream = new MemoryStream())
                {
                    using (var file = File.Open(testFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        file.CopyTo(originalStream);
                    originalStream.Seek(0, SeekOrigin.Begin);
                    var sfo = ParamSfo.ReadFrom(originalStream);
                    var title = sfo.Items.FirstOrDefault(i => i.Key == "TITLE");
                    Assert.That(title, Is.Not.Null);

                    var text = title.StringValue;
                    Assert.That(text, Is.Not.Null);

                    title.StringValue = text;

                    using (var resultStream = new MemoryStream())
                    {
                        sfo.WriteTo(resultStream);
                        var originalBytes = originalStream.ToArray();
                        var resultBytes = resultStream.ToArray();
                        Assert.That(resultBytes, Is.EqualTo(originalBytes), filename);
                    }
                }
            }
        }
    }
}
