using System;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core;
using Umbraco.Core.Logging;

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
                
                try
                {
                    var migrationRunner = new MigrationRunnerProxy(
                                                                    ApplicationContext.Current.ProfilingLogger.Logger,
                                                                    ApplicationContext.Current.Services.MigrationEntryService,
                                                                    ApplicationContext.Current.DatabaseContext);

                    migrationRunner.HandleMigration(
                        Constants.PackageName,
                        new Version("1.0.0"),false); //fire the down code

                }
                catch (Exception ex)
                {
                    LogHelper.Error<MigrationEvents>("Error running DemoPackage migration", ex);
                }
            }
        }
     }
}