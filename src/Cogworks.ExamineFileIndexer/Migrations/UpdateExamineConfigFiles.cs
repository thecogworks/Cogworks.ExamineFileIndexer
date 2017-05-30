using System;
using System.IO;
using System.Web;
using System.Xml.Linq;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Cogworks.ExamineFileIndexer.Migrations
{
    [Migration("1.0.0", 1, "Cogworks.ExamineFileIndexer")]
    public class UpdateExamineConfigFiles : MigrationBase
    {
       
        private readonly string _confDir = HttpContext.Current.Server.MapPath("/config");

        public UpdateExamineConfigFiles(ISqlSyntaxProvider sqlSyntax, ILogger logger) : base(sqlSyntax, logger)
        {

        }

        public override void Up()
        {
            try
            {
                Logger.Debug<UpdateExamineConfigFiles>("looking at dir " + _confDir);
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
            var pathToExamineIndexConfig = Path.Combine(_confDir , "ExamineIndex.config");

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var updatedConfig = configUpdater.UpdateXmlFile(Constants.XpathToTestIndexSectionExists,
                                                            Constants.ExamineIndexFragmentXml, 
                                                            Constants.XpathToInsertIndexSectionAfter);

            updatedConfig.Save(pathToExamineIndexConfig);

            Logger.Debug<UpdateExamineConfigFiles>("Updated examine index config");
        }

        private void UpdateExamineSettingsIndexProviderConfig()
        {
            var pathToExamineIndexConfig = Path.Combine(_confDir, "ExamineSettings.config");

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var updatedConfig = configUpdater.UpdateXmlFile(Constants.XpathToTestIndexProviderSectionExists, 
                                                            Constants.ExamineSettingsProviderFragmentXml, 
                                                            Constants.XpathToInsertIndexProviderSectionAfter);

            updatedConfig.Save(pathToExamineIndexConfig);

            Logger.Debug<UpdateExamineConfigFiles>("Updated examine settings config added index provider");

        }

        private void UpdateExamineSettingsSearchProviderConfig()
        {
            var pathToExamineIndexConfig = Path.Combine(_confDir,"ExamineSettings.config");

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var updatedConfig = configUpdater.UpdateXmlFile(Constants.XpathToTestSearchProviderSectionExists,
                                                            Constants.ExamineSearchProviderFragmentXml, 
                                                            Constants.XpathToInsertSearchProviderSectionAfter);

            updatedConfig.Save(pathToExamineIndexConfig);

            Logger.Debug<UpdateExamineConfigFiles>("Updated examine settings config added search provider");

        }


        private ConfigFileUpdater GetConfigXmlToUpdate(string fileToUpdate)
        {
           
            var indexConfig = XDocument.Load(fileToUpdate);

            var configUpdater = new ConfigFileUpdater(indexConfig);

            return configUpdater;
        }

        public override void Down()
        {
          
            RemoveConfigItem("ExamineIndex.config", Constants.XpathToTestIndexSectionExists);

            RemoveConfigItem("ExamineSettings.config", Constants.XpathToTestIndexProviderSectionExists);

            RemoveConfigItem("ExamineSettings.config", Constants.XpathToTestSearchProviderSectionExists);
        }

        private void RemoveConfigItem(string file,string xPath)
        {
            var pathToExamineIndexConfig = Path.Combine(_confDir, file);

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var configFile = configUpdater.Remove(xPath);

            configFile.Save(pathToExamineIndexConfig);
        }
    }
}
