using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Cogworks.ExamineFileIndexer.Helper;
using Examine;
using Lucene.Net.Analysis;
using TikaOnDotNet.TextExtraction;
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
            YouTubeUrlProperty = String.Empty;
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
            YouTubeUrlProperty = String.Empty;
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
        /// Gets or sets the YouTube umbraco property alias (defaults to String.Empty)
        /// </summary>
        /// <value>The YouTube url property.</value>
        public string YouTubeUrlProperty { get; set; }

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

            //checks if a custom field alias for YouTube urls is specified
            if (!string.IsNullOrEmpty(config["youTubeUrlProperty"]))
                YouTubeUrlProperty = config["youTubeUrlProperty"];
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
                    return (string)x.Attribute("alias") == this.UmbracoFileProperty;
                else
                    return x.Name == this.UmbracoFileProperty;
            });
            //make sure the file exists
            if (filePath != default(XElement) && !string.IsNullOrEmpty((string)filePath))
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
                    DataService.LogService.AddInfoLog((int)node.Attribute("id"), _loggerEntryName + ": No file found at path " + filePath);
                }
            }

            // check if the node has a YouTube url property
            if (!String.IsNullOrEmpty(YouTubeUrlProperty))
            {
                var youTubeProperty = node.Element(YouTubeUrlProperty);
                if (youTubeProperty != null)
                {
                    // get the data from YouTube
                    MetaData.IMetaDataProvider metaDataProvider = new MetaData.YouTubeProvider();
                    var meta = metaDataProvider.GetData(node, youTubeProperty.Value, DataService.LogService);
                    if (meta.Keys.Count > 0)
                    {
                        fields.AddRange(meta);
                    }
                }
            }

            return fields;
        }

        protected override bool ValidateDocument(XElement node)
        {
            if (!IsAllowedExtension(node) && !IsYouTube(node))
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

        /// <summary>
        /// check to see if the YouTube property has been set, and if so does it exist for indexing?
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsYouTube(XElement node)
        {
            if (String.IsNullOrEmpty(YouTubeUrlProperty))
            {
                return false;
            }

            var youTubeProperty = node.Element(YouTubeUrlProperty);
            if (youTubeProperty == null)
                return false;

            return true;
        }

      

        /// <summary>
        /// Parses a media file and extracts the text from it via tika.
        /// </summary>
        public class MediaParser
        {
            /// <summary>
            /// Return only the valid string contents of the file
            /// </summary>
            /// <param name="sourceMedia"></param>
            /// <param name="onError"></param>
            /// <param name="MetaData"> </param>
            /// <returns></returns>
            public string ParseMediaText(string sourceMedia, Action<Exception> onError, out Dictionary<string, string> MetaData)
            {
                var sb = new StringBuilder();
                var metaData = new Dictionary<string, string>();
                var textExtractor = new TextExtractor();
                try
                {
                    var textExtractionResult = textExtractor.Extract(sourceMedia);
                    sb.Append(textExtractionResult.Text);
                    metaData = (Dictionary<string, string>)textExtractionResult.Metadata;
                }
                catch (Exception ex)
                {
                    onError(ex);
                }
                MetaData = metaData;
                return sb.ToString();
            }
        }
    }
}
