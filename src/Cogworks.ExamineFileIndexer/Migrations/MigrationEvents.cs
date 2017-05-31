using System;
using System.IO;
using System.Xml.Linq;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core;

namespace Cogworks.ExamineFileIndexer.Migrations
{
    public class MigrationEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(
            UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            var migrationRunner = new MigrationRunnerProxy(
                applicationContext.ProfilingLogger.Logger,
                applicationContext.Services.MigrationEntryService,
                applicationContext.DatabaseContext);

            migrationRunner.HandleMigration(
                Constants.PackageName,
                new Version("1.0.0"));
            
            InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;
        }

        private void InstalledPackage_BeforeDelete(InstalledPackage sender, EventArgs e)
        {
            if (sender.Data.Name == Constants.PackageName)
            {
                RemoveConfigItem(Constants.ExamineIndexConfig, Constants.XpathToTestIndexSectionExists);

                RemoveConfigItem(Constants.ExamineSettingsConfig, Constants.XpathToTestIndexProviderSectionExists);

                RemoveConfigItem(Constants.ExamineSettingsConfig, Constants.XpathToTestSearchProviderSectionExists);

                ApplicationContext.Current.ProfilingLogger.Logger.Debug(GetType(), "Removing config entries for " + Constants.PackageName);

                ApplicationContext.Current.ProfilingLogger.Logger.Debug(GetType(), "Removing migration for " + Constants.PackageName);

                RemoveEntryFromMigrationTable();
            }
        }

        private void RemoveEntryFromMigrationTable()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            db.Execute("delete from umbracoMigration where name='{0}'", Constants.PackageName);      
        }

        private void RemoveConfigItem(string file, string xPath)
        {
            var pathToExamineIndexConfig = Path.Combine(UpdateExamineConfigFiles.ConfDir, file);

            var configUpdater = GetConfigXmlToUpdate(pathToExamineIndexConfig);

            var configFile = configUpdater.Remove(xPath);

            configFile.Save(pathToExamineIndexConfig);
        }

        private ConfigFileUpdater GetConfigXmlToUpdate(string fileToUpdate)
        {

            var indexConfig = XDocument.Load(fileToUpdate);

            var configUpdater = new ConfigFileUpdater(indexConfig);

            return configUpdater;
        }
    }
}