namespace Cogworks.ExamineFileIndexer
{
    public class Constants
    {
        public const string PackageName = "Cogworks.ExamineFileIndexer";

        public const string ExamineIndexConfig = "ExamineIndex.config";

        public const string ExamineSettingsConfig = "ExamineSettings.config";
        
        public const string ExamineIndexFragmentXml= "<IndexSet SetName=\"MediaIndexSet\" IndexPath=\"~/App_Data/TEMP/ExamineIndexes/MediaIndexSet\"><IndexAttributeFields><add Name=\"id\" /><add Name=\"nodeName\" /><add Name=\"updateDate\" /><add Name=\"writerName\"/><add Name=\"path\" /><add Name=\"nodeTypeAlias\" /><add Name=\"parentID\" /></IndexAttributeFields><IncludeNodeTypes><add Name=\"File\" /></IncludeNodeTypes></IndexSet>";

        public const string XpathToTestIndexSectionExists = "/ExamineLuceneIndexSets/IndexSet[@SetName='MediaIndexSet']";
        
        public const string XpathToInsertIndexSectionAfter = "/ExamineLuceneIndexSets/IndexSet";

        public const string XpathToTestIndexProviderSectionExists = "/Examine/ExamineIndexProviders/providers/add[@name='MediaIndexer']";

        public const string XpathToInsertIndexProviderSectionAfter = "/Examine/ExamineIndexProviders/providers/add";

        public const string ExamineSettingsProviderFragmentXml =
            "<add name=\"MediaIndexer\" type=\"Cogworks.ExamineFileIndexer.UmbracoMediaFileIndexer, Cogworks.ExamineFileIndexer\" extensions=\".pdf,.docx\" umbracoFileProperty=\"umbracoFile\"/>";


        public const string XpathToTestSearchProviderSectionExists = "/Examine/ExamineSearchProviders/providers/add[@name='MediaSearcher']";

        public const string XpathToInsertSearchProviderSectionAfter = "/Examine/ExamineSearchProviders/providers/add";

        public const string ExamineSearchProviderFragmentXml =
            " <add name=\"MediaSearcher\" type=\"UmbracoExamine.UmbracoExamineSearcher, UmbracoExamine\" indexSet=\"MediaIndexSet\" analyzer=\"Lucene.Net.Analysis.Standard.StandardAnalyzer, Lucene.Net\"/>";
    }
}