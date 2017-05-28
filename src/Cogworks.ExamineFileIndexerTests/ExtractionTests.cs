using System;
using System.Collections.Generic;
using System.IO;
using Cogworks.ExamineFileIndexer;
using Cogworks.ExamineFileIndexerTests.Helper;
using NUnit.Framework;

namespace Cogworks.ExamineFileIndexerTests
{
    [TestFixture]    
    public class ExtractionTests
    {
        [Test]
        public void Given_WordFile_Expect_Extracted_Content()
        {
            string wordFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.DocToTest);

            var umbracoFileIndexer=new MediaParser();

            var metaData=new Dictionary<string,string>();

            string extractedText = umbracoFileIndexer.ParseMediaText(wordFileToTest, WriteToConsole, out metaData);

         
            Assert.IsTrue(extractedText.Contains("Current"));

        }

        [Test]
        public void Given_PdfFile_Expect_Extracted_Content()
        {
            string pdfFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.PdfToTest);

            var umbracoFileIndexer = new MediaParser();

            var metaData = new Dictionary<string, string>();

            string extractedText = umbracoFileIndexer.ParseMediaText(pdfFileToTest, WriteToConsole, out metaData);

            Assert.IsTrue(extractedText.Contains("PowerShell"));
        }

        [Test]
        [TestCase]
        public void Given_Large_No_Of_Docs_Expect_No_Out_Of_Memory_Exceptions_Thrown()
        {
            string pdfFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.PdfToTest);

            int noOfDocs = 1000;

            var umbracoFileIndexer = new MediaParser();

            var metaData = new Dictionary<string, string>();

            for (int i = 0; i < noOfDocs; i++)
            {
                string extractedText = umbracoFileIndexer.ParseMediaText(pdfFileToTest, WriteToConsole, out metaData);

                Assert.IsTrue(extractedText.Contains("PowerShell"));
            }
        }

        private void WriteToConsole(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }
}
