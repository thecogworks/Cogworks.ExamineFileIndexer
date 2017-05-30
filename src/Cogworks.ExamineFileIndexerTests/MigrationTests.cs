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

            ConfigFileUpdateMigration updater = new ConfigFileUpdateMigration(xmlFile);  

            string xmlElementToInsert = Constants.ExamineIndexFragmentXml;

            XDocument updateDocument = updater.UpdateXmlFile(xpathToTestSectionExists, xmlElementToInsert, Constants.XpathToInsertIndexSectionAfter);

            int nodeCountAfterUpdate = updateDocument.XPathSelectElements(xpathToTestSectionExists).Count();

            Assert.AreNotEqual(initialNodeCode,nodeCountAfterUpdate);
            
        }

        [Test]
        public void Given_Examine_SettingsFile_Add_MediaIndexer_Searcher_To_Config()
        {
            string pathToConfig = Path.Combine(TestContext.CurrentContext.TestDirectory, TestHelper.ExamineSettingsConfigFile);

            XDocument xmlFile = XDocument.Load(pathToConfig);

            int initialNodeCode = xmlFile.XPathSelectElements(Constants.XpathToTestIndexProviderSectionExists).Count();

            ConfigFileUpdateMigration updater = new ConfigFileUpdateMigration(xmlFile);

            XDocument updateDocument = updater.UpdateXmlFile(Constants.XpathToTestIndexProviderSectionExists,
                Constants.ExamineSettingsProviderFragmentXml, Constants.XpathToInsertIndexProviderSectionAfter);


            int nodeCountAfterUpdate = updateDocument.XPathSelectElements(Constants.XpathToTestIndexProviderSectionExists).Count();

            Assert.AreNotEqual(initialNodeCode, nodeCountAfterUpdate);
        }
    }

    
}
