using System.Collections.Generic;
using System.Xml.Linq;
using UmbracoExamine.DataServices;

namespace Cogworks.ExamineFileIndexer.MetaData
{
    public interface IMetaDataProvider
    {
        Dictionary<string, string> GetData(XElement node, string url, ILogService log);
    }
}
