using System.IO;
using Cogworks.ExamineFileIndexer;
using NUnit.Framework;

namespace Cogworks.ExamineFileIndexerTests
{
    [TestFixture]
    public class ExamineFileIndexerTests
    {
        [Test]
        public void Given_Valid_File_ExpectExtractedContent()
        {
            var indexer = new TextUmbracoFileIndexer { SupportedExtensions = new[] {".docx"}};

            string wordFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestFiles\IntegratingMVCapplicationsintoUmbraco.docx");

            string extractedText = indexer.Extract(wordFileToTest);

            Assert.IsTrue(extractedText.Contains("Agenda"));
        }

        [Test]
        public void Given_Valid_PdfFile_And_Pdf_Not_Allowed_Expect_SystemNotSupportedException()
        {
            var indexer = new TextUmbracoFileIndexer { SupportedExtensions = new[] { ".docx" } };

            string wordFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestFiles\Exploring_PowerShell_Automation.pdf");

            var ex = Assert.Throws<System.NotSupportedException>(()=> indexer.Extract(wordFileToTest));

            Assert.IsTrue(ex.Message.Contains("The file with the extension specified is not supported"));
        }

        /// <summary>
        /// the method we want to test is protected so we fake an instance and test with that instead
        /// </summary>
        public class TextUmbracoFileIndexer : UmbracoMediaFileIndexer
        {
            public string Extract(string file)
            {
                FileInfo fileInfo = new FileInfo(file);
                return ExtractTextFromFile(fileInfo);
            }
        }
    }
}
