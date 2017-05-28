using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;
using Cogworks.ExamineFileIndexer.Helper;
using Examine;
using Lucene.Net.Analysis;
using UmbracoExamine;
using UmbracoExamine.DataServices;

namespace Cogworks.ExamineFileIndexer
{
    /// <summary>
    /// 
    /// </summary>
    public class UmbracoMediaFileIndexer : BaseUmbracoIndexer
    {

        public Dictionary<string, string> ExtractedMetaFromTika => _extractedMetaFromTika;

        private Dictionary<string, string> _extractedMetaFromTika = new Dictionary<string, string>();

        private readonly string _loggerEntryName = typeof(UmbracoMediaFileIndexer).Name;

        public UmbracoMediaFileIndexer()
        {
            SupportedExtensions = new[] { ".pdf" };
            UmbracoFileProperty = "umbracoFile";
        }

        /// <summary>
        /// Constructor to allow for creating an indexer at runtime
        /// </summary>
        /// <param name="indexPath"></param>
        /// <param name="dataService"></param>
        /// <param name="analyzer"></param>
        /// <param name="async"></param>
        public UmbracoMediaFileIndexer(DirectoryInfo indexPath, IDataService dataService, Analyzer analyzer, bool async)
            : base(
                new IndexCriteria(Enumerable.Empty<IIndexField>(), Enumerable.Empty<IIndexField>(), Enumerable.Empty<string>(), Enumerable.Empty<string>(), null),
                indexPath, dataService, analyzer, async)
        {
            SupportedExtensions = new[] { ".pdf" };
            UmbracoFileProperty = "umbracoFile";
        }


        /// <summary>
        /// Gets or sets the supported extensions for files
        /// see http://tika.apache.org/1.2/formats.html for supported formats
        /// </summary>
        /// <value>The supported extensions.</value>
        public IEnumerable<string> SupportedExtensions { get; set; }

        /// <summary>
        /// Gets or sets the umbraco property alias (defaults to umbracoFile)
        /// </summary>
        /// <value>The umbraco file property.</value>
        public string UmbracoFileProperty { get; set; }

       

        /// <summary>
        /// Gets the name of the Lucene.Net field which the content is inserted into
        /// </summary>
        /// <value>The name of the text content field.</value>
        public const string TextContentFieldName = "FileTextContent";

        protected override IEnumerable<string> SupportedTypes
        {
            get
            {
                return new string[] { IndexTypes.Media };
            }
        }

        /// <summary>
        /// Set up all properties for the indexer based on configuration information specified. This will ensure that
        /// all of the folders required by the indexer are created and exist. This will also create an instruction
        /// file declaring the computer name that is part taking in the indexing. This file will then be used to
        /// determine the master indexer machine in a load balanced environment (if one exists).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            if (!string.IsNullOrEmpty(config["extensions"]))
                SupportedExtensions = config["extensions"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //checks if a custom field alias is specified
            if (!string.IsNullOrEmpty(config["umbracoFileProperty"]))
                UmbracoFileProperty = config["umbracoFileProperty"];

        }

        /// <summary>
        /// Provides the means to extract the text to be indexed from the file specified
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual string ExtractTextFromFile(FileInfo file)
        {
            if (!SupportedExtensions.Select(x => x.ToUpper()).Contains(file.Extension.ToUpper()))
            {
                throw new NotSupportedException("The file with the extension specified is not supported");
            }

            var mediaParser = new MediaParser();

            Action<Exception> onError = (e) => OnIndexingError(new IndexingErrorEventArgs("Could not read media item", -1, e));

            var txt = mediaParser.ParseMediaText(file.FullName, onError, out _extractedMetaFromTika);

            return txt;

        }

        /// <summary>
        /// Collects all of the data that needs to be indexed as defined in the index set.
        /// </summary>
        /// <param name="node">Media item XML being indexed</param>
        /// <param name="type">Type of index (should only ever be media)</param>
        /// <returns>Fields containing the data for the index</returns>
        protected override Dictionary<string, string> GetDataToIndex(XElement node, string type)
        {

            var fields = base.GetDataToIndex(node, type);

            //find the field which contains the file
            var filePath = node.Elements().FirstOrDefault(x =>
            {
                if (x.Attribute("alias") != null)
                {
                    return (string)x.Attribute("alias") == this.UmbracoFileProperty;
                }
                else
                {
                    return x.Name == this.UmbracoFileProperty;
                }
            });

            
            if (FileExists(filePath))
            {
                //get the file path from the data service
                var fullPath = this.DataService.MapPath((string)filePath);
                var fi = new FileInfo(fullPath);
                if (fi.Exists)
                {
                    try
                    {
                        fields.Add(TextContentFieldName, ExtractTextFromFile(fi));

                        //add any addtional meta data extracted via tika from MediaParser.ParseMediaText
                        fields.AddRange(_extractedMetaFromTika);
                    }
                    catch (NotSupportedException)
                    {
                        //log that we couldn't index the file found
                        DataService.LogService.AddErrorLog((int)node.Attribute("id"), _loggerEntryName + ": Extension '" + fi.Extension + "' is not supported at this time");
                    }
                }
                else
                {
                    // perhaps it's available via the VirtualPathProvider ?
                    var stream = VirtualPathProvider.OpenFile((string)filePath);

                    if (stream.CanRead)
                    {
                        var extractionResult = ExtractContentFromStream(stream);

                        if (!string.IsNullOrWhiteSpace(extractionResult.ExtractedText))
                        {
                            fields.Add(TextContentFieldName, extractionResult.ExtractedText);
                            fields.AddRange(extractionResult.MetaData);
                        }
                    }
                    else
                    {
                        DataService.LogService.AddInfoLog((int)node.Attribute("id"), _loggerEntryName + ": No file found at path " + filePath);
                    }
                }
            }

            return fields;
        }

        private ExtractionResult ExtractContentFromStream(Stream stream)
        {
            byte[] data;
            var metaData = new Dictionary<string, string>();

            var extractionResult = new ExtractionResult();

            Action<Exception> onError = (e) => OnIndexingError(new IndexingErrorEventArgs("Could not read media item", -1, e));

            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                data = ms.ToArray();
            }

            if (data != null && data.Length>0)
            {
                var mediaParser = new MediaParser();
                extractionResult.ExtractedText = mediaParser.ParseMediaText(data,onError, out metaData);
                extractionResult.MetaData = metaData;
            }

            return extractionResult;
        }

        private static bool FileExists(XElement filePath)
        {
            return filePath != default(XElement) && !string.IsNullOrEmpty((string)filePath);
        }

        protected override bool ValidateDocument(XElement node)
        {
            if (!IsAllowedExtension(node))
            {
                return false;
            }
            return base.ValidateDocument(node);
        }

        /// <summary>
        /// need to prevent extensions not in allowed extensions getting into the index
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsAllowedExtension(XElement node)
        {
            var extension = node.Element("umbracoExtension");

            if (extension != null)
            {
                var extensionValue = "." + extension.Value;
                if (SupportedExtensions.Select(x => x.ToUpper()).Contains(extensionValue.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }

        internal struct ExtractionResult
        {
            public string ExtractedText { get; set; }
            public Dictionary<string,string> MetaData { get; set; }
        }
    }
}
