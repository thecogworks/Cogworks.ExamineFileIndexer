using System;
using System.Xml.Linq;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Cogworks.ExamineFileIndexer.Migrations
{
    [Migration("1.0.0", 1, "Cogworks.ExamineFileIndexer")]
    public class UpdateExamineConfigFiles : MigrationBase
    {
        public UpdateExamineConfigFiles(ISqlSyntaxProvider sqlSyntax, ILogger logger) : base(sqlSyntax, logger)
        {

        }

        public override void Up()
        {
            try
            {
                UpdateExamineIndexConfig();

                UpdateExamineSettingsIndexProviderConfig();

                UpdateExamineSettingsSearchProviderConfig();
            }
            catch (Exception ex)
            {
                Logger.Error<Exception>("Error updating examine config files", ex);
            }
        }

        private void UpdateExamineIndexConfig()
        {
            var pathToExamineIndexConfig = SystemDirectories.Config + "/ExamineIndex.config";

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var updatedConfig = configUpdater.UpdateXmlFile(Constants.XpathToTestIndexSectionExists,
                                                            Constants.ExamineIndexFragmentXml, 
                                                            Constants.XpathToInsertIndexSectionAfter);

            updatedConfig.Save(pathToExamineIndexConfig);
        }

        private void UpdateExamineSettingsIndexProviderConfig()
        {
            var pathToExamineIndexConfig = SystemDirectories.Config + "/ExamineSettings.config";

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var updatedConfig = configUpdater.UpdateXmlFile(Constants.XpathToTestIndexProviderSectionExists, 
                                                            Constants.ExamineSettingsProviderFragmentXml, 
                                                            Constants.XpathToInsertIndexProviderSectionAfter);

            updatedConfig.Save(pathToExamineIndexConfig);

        }

        private void UpdateExamineSettingsSearchProviderConfig()
        {
            var pathToExamineIndexConfig = SystemDirectories.Config + "/ExamineSettings.config";

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var updatedConfig = configUpdater.UpdateXmlFile(Constants.XpathToTestSearchProviderSectionExists,
                                                            Constants.ExamineSearchProviderFragmentXml, 
                                                            Constants.XpathToInsertSearchProviderSectionAfter);

            updatedConfig.Save(pathToExamineIndexConfig);

        }


        private ConfigFileUpdater GetConfigXmlToUpdate(string fileToUpdate)
        {
            var pathToExamineIndexConfig = IOHelper.MapPath(SystemDirectories.Config + fileToUpdate);

            var indexConfig = XDocument.Load(pathToExamineIndexConfig);

            var configUpdater = new ConfigFileUpdater(indexConfig);

            return configUpdater;
        }

        public override void Down()
        {
            
        }
    }
}
