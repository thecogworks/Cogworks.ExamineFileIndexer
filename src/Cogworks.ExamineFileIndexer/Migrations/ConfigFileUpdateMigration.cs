using System.Xml.Linq;
using System.Xml.XPath;

namespace Cogworks.ExamineFileIndexer.Migrations
{
    public class ConfigFileUpdateMigration
    {
        private XDocument _xmlFile;

        public ConfigFileUpdateMigration(XDocument xmlFile)
        {
            _xmlFile = xmlFile;

        }

        public XDocument UpdateXmlFile(string xpathToTestSectionExists, string xmlElementToInsert, string insertAfter)
        {
            var elementToAdd = XElement.Parse(xmlElementToInsert);

            var item = _xmlFile.XPathSelectElement(insertAfter);

            item.Parent.Add(elementToAdd);

            return _xmlFile;
        }
    }
}
