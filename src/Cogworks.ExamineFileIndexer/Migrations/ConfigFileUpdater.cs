using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Cogworks.ExamineFileIndexer.Migrations
{
    public class ConfigFileUpdater
    {
        private XDocument _xmlFile;

        public ConfigFileUpdater(XDocument xmlFile)
        {
            _xmlFile = xmlFile;

        }

        public XDocument UpdateXmlFile(string xpathToTestSectionExists, string xmlElementToInsert, string insertAfter)
        {
            var existingNode = _xmlFile.XPathSelectElements(xpathToTestSectionExists);

            if (!existingNode.Any())
            {
                var elementToAdd = XElement.Parse(xmlElementToInsert);

                var item = _xmlFile.XPathSelectElement(insertAfter);

                item.Parent.Add(elementToAdd);

            }

            return _xmlFile;
        }
    }
}
