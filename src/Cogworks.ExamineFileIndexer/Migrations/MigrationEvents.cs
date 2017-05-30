using System;
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
                "Cogworks.ExamineFileIndexer",
                new Version("1.0.0"));

          
        }
    }
}