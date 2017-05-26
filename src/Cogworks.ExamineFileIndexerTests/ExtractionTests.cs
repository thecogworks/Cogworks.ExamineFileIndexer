using System;
using System.Collections.Generic;
using System.IO;
using Cogworks.ExamineFileIndexer;
using NUnit.Framework;

namespace Cogworks.ExamineFileIndexerTests
{
    [TestFixture]    
    public class ExtractionTests
    {
        [Test]
        public void Given_WordFile_Expect_Extracted_Content()
        {
            string wordFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestFiles\IntegratingMVCapplicationsintoUmbraco.docx");

            var umbracoFileIndexer=new UmbracoMediaFileIndexer.MediaParser();

            var metaData=new Dictionary<string,string>();

            string extractedText = umbracoFileIndexer.ParseMediaText(wordFileToTest, WriteToConsole, out metaData);

         
            Assert.IsTrue(extractedText.Contains("Agenda"));

        }

        [Test]
        public void Given_PdfFile_Expect_Extracted_Content()
        {
            string pdfFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestFiles\Exploring_PowerShell_Automation.pdf");

            var umbracoFileIndexer = new UmbracoMediaFileIndexer.MediaParser();

            var metaData = new Dictionary<string, string>();

            string extractedText = umbracoFileIndexer.ParseMediaText(pdfFileToTest, WriteToConsole, out metaData);

            Assert.IsTrue(extractedText.Contains("PowerShell"));
        }

        private void WriteToConsole(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }
}
