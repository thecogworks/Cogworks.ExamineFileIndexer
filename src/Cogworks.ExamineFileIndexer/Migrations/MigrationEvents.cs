using System;
using Semver;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Web;

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
                new Version(Constants.Version));
            
            InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;
        }

        private void InstalledPackage_BeforeDelete(InstalledPackage sender, EventArgs e)
        {
            if (sender.Data.Name == Constants.PackageName)
            {
                try
                {
                    
                    var mes = ApplicationContext.Current.Services.MigrationEntryService;
                    var logger = ApplicationContext.Current.ProfilingLogger.Logger;

                    var migrationsRunner = new MigrationRunner(
                        mes,
                        logger,
                        new SemVersion(0),
                        new SemVersion(Constants.VersionNo), 
                        Constants.PackageName);

                        var db = UmbracoContext.Current.Application.DatabaseContext.Database;

                        //calls the down method on migration UpdateExamineConfigFiles however the db entry for migration is not removed
                        //need to do that manually
                        migrationsRunner.Execute(db, false); 

                        RemoveMigrationFromDb(db);
                   
                }
                catch (Exception ex)
                {
                    LogHelper.Error<MigrationEvents>("Error running DemoPackage migration", ex);
                }
            }
        }

        private void RemoveMigrationFromDb(UmbracoDatabase db)
        {
            using (Transaction transaction = db.GetTransaction())
            {
                db.Execute("delete from umbracoMigration where version='{0}'", Constants.Version);
                transaction.Complete();
            }
        }
    }
}