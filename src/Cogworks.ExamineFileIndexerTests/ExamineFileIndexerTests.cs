using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Cogworks.ExamineFileIndexer;
using Cogworks.ExamineFileIndexerTests.Helper;
using Examine.LuceneEngine;
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

            string wordFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.DocToTest);

            string extractedText = indexer.Extract(wordFileToTest);

            Assert.IsTrue(extractedText.Contains("Current"));
        }

        [Test]
        public void Given_Valid_File_ExpectMeta_After_Extraction()
        {
            var indexer = new TextUmbracoFileIndexer { SupportedExtensions = new[] { ".pdf" } };

            string pdfFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory,TestHelper.PdfToTest);

           indexer.Extract(pdfFileToTest);

            var metaData = indexer.ExtractedMetaFromTika;

           Assert.AreEqual(metaData["dc:creator"], "Dragana");
        }

        [Test]
        public void Given_Valid_PdfFile_And_Pdf_Not_Allowed_Expect_SystemNotSupportedException()
        {
            var indexer = new TextUmbracoFileIndexer { SupportedExtensions = new[] { ".docx" } };

            string wordFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.PdfToTest);

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
