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
        private string _confDir = SystemDirectories.Config.Replace("config~\\",string.Empty);

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
            var pathToExamineIndexConfig = _confDir + "ExamineIndex.config";

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var updatedConfig = configUpdater.UpdateXmlFile(Constants.XpathToTestIndexSectionExists,
                                                            Constants.ExamineIndexFragmentXml, 
                                                            Constants.XpathToInsertIndexSectionAfter);

            updatedConfig.Save(pathToExamineIndexConfig);

            Logger.Debug<UpdateExamineConfigFiles>("Updated examine index config");
        }

        private void UpdateExamineSettingsIndexProviderConfig()
        {
            var pathToExamineIndexConfig = _confDir + "/ExamineSettings.config";

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var updatedConfig = configUpdater.UpdateXmlFile(Constants.XpathToTestIndexProviderSectionExists, 
                                                            Constants.ExamineSettingsProviderFragmentXml, 
                                                            Constants.XpathToInsertIndexProviderSectionAfter);

            updatedConfig.Save(pathToExamineIndexConfig);

            Logger.Debug<UpdateExamineConfigFiles>("Updated examine settings config added index provider");

        }

        private void UpdateExamineSettingsSearchProviderConfig()
        {
            var pathToExamineIndexConfig = _confDir + "/ExamineSettings.config";

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var updatedConfig = configUpdater.UpdateXmlFile(Constants.XpathToTestSearchProviderSectionExists,
                                                            Constants.ExamineSearchProviderFragmentXml, 
                                                            Constants.XpathToInsertSearchProviderSectionAfter);

            updatedConfig.Save(pathToExamineIndexConfig);

            Logger.Debug<UpdateExamineConfigFiles>("Updated examine settings config added search provider");

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
            //todo add removal code
        }
    }
}
