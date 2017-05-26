using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using UmbracoExamine.DataServices;

namespace Cogworks.ExamineFileIndexer.MetaData
{
    public class YouTubeProvider : IMetaDataProvider
    {
        private const string LoggerEntryName = "CogUmbracoExamineMediaIndexer.MetaData.YouTubeProvider";
        private const string YouTubeApiTemplate = "http://gdata.youtube.com/feeds/api/videos/{0}";
        private const string YouTubeVideoIdentifier = "youtube.com/watch";

        /// <summary>
        /// Gets the YouTube meta data for a video
        /// </summary>
        /// <param name="node">The node containing the url for the video</param>
        /// <param name="url">The standard YouTube video url</param>
        /// <returns>An array of title/content meta data strings</returns>
        public Dictionary<string, string> GetData(XElement node, string url, ILogService log)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            WebClient http = new WebClient();
            string rawXml = string.Empty;

            // convert the standard YouTube url into a rest api call url
            string apiUrl = convertStandardUrlToApi(url);
            if (string.IsNullOrEmpty(apiUrl))
            {
                return results;
            }

            // attempt to download the meta data for the video from YouTube
            try
            {
                rawXml = http.DownloadString(apiUrl);
            }
            catch (WebException ex)
            {
                log.AddErrorLog((int)node.Attribute("id"), LoggerEntryName + ": " + ex.Message);
                return results;
            }

            // if something went wrong, abandon
            if (string.IsNullOrEmpty(rawXml))
            {
                return results;
            }

            // attempt to parse the response from YouTube into an Xml document.
            XDocument xml = null;
            try
            {
                xml = XDocument.Parse(rawXml);
            }
            catch (System.Xml.XmlException ex)
            {
                log.AddErrorLog((int)node.Attribute("id"), LoggerEntryName + ": " + ex.Message);
                return results;
            }

            // the only 2 fields in this lot we are interested in is Title/Content which have text in.
            XNamespace ns = @"http://www.w3.org/2005/Atom";
            var entry = xml.Element(ns + "entry");

            // title
            string title = (from t in xml.Element(ns + "entry").Descendants(ns + "title")
                            select t).FirstOrDefault().Value;
            if (!String.IsNullOrEmpty(title))
            {
                results.Add("youtubeTitle", title);
            }

            // content
            string content = (from t in xml.Element(ns + "entry").Descendants(ns + "content")
                              select t).FirstOrDefault().Value;
            if (!String.IsNullOrEmpty(content))
            {
                results.Add("youtubeContent", content);
            }
            return results;
        }

        /// <summary>
        /// This converts a standard YouTube url "http://www.youtube.com/watch?v=*******" to a 
        /// REST api call for the same video
        /// </summary>
        /// <param name="standardUrl"></param>
        /// <returns></returns>
        private String convertStandardUrlToApi(string standardUrl)
        {
            if (!standardUrl.Contains(YouTubeVideoIdentifier))
            {
                return string.Empty;
            }

            Int32 indexA = standardUrl.IndexOf("v=");
            if (indexA == -1)
            {
                return string.Empty;
            }

            Int32 indexB = standardUrl.IndexOf("&", indexA);
            if (indexB == -1)
            {
                return string.Format(YouTubeApiTemplate, standardUrl.Substring(indexA + 2));
            }
            else
            {
                return string.Format(YouTubeApiTemplate, standardUrl.Substring(indexA + 2, indexB - (indexA + 2)));
            }
        }
    }
}
