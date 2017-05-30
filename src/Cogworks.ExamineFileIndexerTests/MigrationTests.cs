using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Cogworks.ExamineFileIndexer;
using Cogworks.ExamineFileIndexer.Migrations;
using Cogworks.ExamineFileIndexerTests.Helper;
using NUnit.Framework;
namespace Cogworks.ExamineFileIndexerTests
{
    [TestFixture]
    public class MigrationTests
    {
        [Test]
        public void Given_Examine_IndexFile_Add_MediaIndex_To_Config()
        {
            string pathToConfig = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.ExamineIndexConfigFile);

            XDocument xmlFile =XDocument.Load(pathToConfig);

            string xpathToTestSectionExists = Constants.XpathToTestIndexSectionExists;

            int initialNodeCode = xmlFile.XPathSelectElements(xpathToTestSectionExists).Count();

            ConfigFileUpdater updater = new ConfigFileUpdater(xmlFile);  

            string xmlElementToInsert = Constants.ExamineIndexFragmentXml;

            XDocument updateDocument = updater.UpdateXmlFile(xpathToTestSectionExists, xmlElementToInsert, Constants.XpathToInsertIndexSectionAfter);

            int nodeCountAfterUpdate = updateDocument.XPathSelectElements(xpathToTestSectionExists).Count();

            Assert.AreNotEqual(initialNodeCode,nodeCountAfterUpdate);
            
        }

        [Test]
        public void Given_Examine_IndexFile_With_Media_Index_Expect_Another_Media_Index_To_Not_Add()
        {
            string pathToConfig = Path.Combine(TestContext.CurrentContext.TestDirectory, @"config\ExamineIndexWithMediaIndex.config");

            XDocument xmlFile = XDocument.Load(pathToConfig);

            string xpathToTestSectionExists = Constants.XpathToTestIndexSectionExists;

            int initialNodeCode = xmlFile.XPathSelectElements(xpathToTestSectionExists).Count(); //expect it to be 1

            ConfigFileUpdater updater = new ConfigFileUpdater(xmlFile);

            string xmlElementToInsert = Constants.ExamineIndexFragmentXml;

            XDocument updateDocument = updater.UpdateXmlFile(xpathToTestSectionExists, xmlElementToInsert, Constants.XpathToInsertIndexSectionAfter);

            int nodeCountAfterUpdate = updateDocument.XPathSelectElements(xpathToTestSectionExists).Count();

            Assert.AreEqual(initialNodeCode, nodeCountAfterUpdate);

        }

        [Test]
        public void Given_Examine_SettingsFile_Add_MediaIndexer_To_Config()
        {
            string pathToConfig = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.ExamineSettingsConfigFile);

            XDocument xmlFile = XDocument.Load(pathToConfig);

            int initialNodeCode = xmlFile.XPathSelectElements(Constants.XpathToTestIndexProviderSectionExists).Count();

            ConfigFileUpdater updater = new ConfigFileUpdater(xmlFile);

            XDocument updateDocument = updater.UpdateXmlFile(Constants.XpathToTestIndexProviderSectionExists,
                Constants.ExamineSettingsProviderFragmentXml, Constants.XpathToInsertIndexProviderSectionAfter);


            int nodeCountAfterUpdate = updateDocument.XPathSelectElements(Constants.XpathToTestIndexProviderSectionExists).Count();

            Assert.AreNotEqual(initialNodeCode, nodeCountAfterUpdate);
        }

        [Test]
        public void Given_Examine_SettingsFile_Add_Searcher_To_Config()
        {
            string pathToConfig = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.ExamineSettingsConfigFile);

            XDocument xmlFile = XDocument.Load(pathToConfig);

            int initialNodeCode = xmlFile.XPathSelectElements(Constants.XpathToTestSearchProviderSectionExists).Count();

            ConfigFileUpdater updater = new ConfigFileUpdater(xmlFile);

            XDocument updateDocument = updater.UpdateXmlFile(Constants.XpathToTestSearchProviderSectionExists,
                Constants.ExamineSearchProviderFragmentXml, Constants.XpathToInsertSearchProviderSectionAfter);


            int nodeCountAfterUpdate = updateDocument.XPathSelectElements(Constants.XpathToTestSearchProviderSectionExists).Count();

            Assert.AreNotEqual(initialNodeCode, nodeCountAfterUpdate);
        }

        [Test]
        public void When_XPath_And_Remove_Executed_Expect_Element_To_Be_Removed()
        {
            string pathToConfig = Path.Combine(TestContext.CurrentContext.TestDirectory, @"config\ExamineIndexWithMediaIndex.config");

            XDocument xmlFile = XDocument.Load(pathToConfig);

            string xpathToRemove = Constants.XpathToTestIndexSectionExists;

            ConfigFileUpdater updater = new ConfigFileUpdater(xmlFile);

            XDocument updatedDocument = updater.Remove(xpathToRemove);

            int nodeCountAfterUpdate = updatedDocument.XPathSelectElements(xpathToRemove).Count();

            Assert.AreEqual(0,nodeCountAfterUpdate);
        }
    }

    
}
