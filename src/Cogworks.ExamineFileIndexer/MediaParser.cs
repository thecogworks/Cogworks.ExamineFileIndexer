using System;
using System.Collections.Generic;
using System.Text;
using TikaOnDotNet.TextExtraction;

namespace Cogworks.ExamineFileIndexer
{
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

        public string ParseMediaText(byte[] data,Action<Exception> onError, out Dictionary<string, string> MetaData)
        {
            TextExtractor textExtractor = new TextExtractor();
            var metaData = new Dictionary<string, string>();
            var sb = new StringBuilder();
            try
            {
                TextExtractionResult textExtractionResult = textExtractor.Extract(data);

                if (!string.IsNullOrWhiteSpace(textExtractionResult.Text))
                {
                    metaData = (Dictionary<string, string>)textExtractionResult.Metadata;
                    
                    sb.Append(textExtractionResult.Text);
                }
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