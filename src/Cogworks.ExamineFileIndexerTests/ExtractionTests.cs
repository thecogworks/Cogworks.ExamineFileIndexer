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

            var mediaParser=new MediaParser();

            var metaData=new Dictionary<string,string>();

            string extractedText = mediaParser.ParseMediaText(wordFileToTest, WriteToConsole, out metaData);

         
            Assert.IsTrue(extractedText.Contains("Current"));

        }

        [Test]
        public void Given_PdfFile_Expect_Extracted_Content()
        {
            string pdfFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.PdfToTest);

            var mediaParser = new MediaParser();

            var metaData = new Dictionary<string, string>();

            string extractedText = mediaParser.ParseMediaText(pdfFileToTest, WriteToConsole, out metaData);

            Assert.IsTrue(extractedText.Contains("PowerShell"));
        }

        [Test]
        public void Given_Pdf_Stream_Expect_Extracted_Content()
        {
            string pdfFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.PdfToTest);

            byte[] data = FileToByteArray(pdfFileToTest);

            var metaData = new Dictionary<string, string>();

            var mediaParser = new MediaParser();

            var extractedText = mediaParser.ParseMediaText(data, WriteToConsole, out metaData);

            Assert.IsTrue(extractedText.Contains("PowerShell"));
        }

        private byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        [Test]
        [TestCase]
        public void Given_Large_No_Of_Docs_Expect_No_Out_Of_Memory_Exceptions_Thrown()
        {
            string pdfFileToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.PdfToTest);

            int noOfDocs = 1000;

            var mediaParser = new MediaParser();

            var metaData = new Dictionary<string, string>();

            for (int i = 0; i < noOfDocs; i++)
            {
                string extractedText = mediaParser.ParseMediaText(pdfFileToTest, WriteToConsole, out metaData);

                Assert.IsTrue(extractedText.Contains("PowerShell"));
            }
        }

        private void WriteToConsole(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }
}
