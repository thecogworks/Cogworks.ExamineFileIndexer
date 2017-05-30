using System;
using System.Linq;
using Semver;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Services;

namespace Cogworks.ExamineFileIndexer.Migrations
{
    public class MigrationRunnerProxy
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger _logger;
        private readonly IMigrationEntryService _migrationEntryService;

        public MigrationRunnerProxy(
            ILogger logger,
            IMigrationEntryService migrationEntryService,
            DatabaseContext umbracoContext)
        {
            _logger = logger;
            _migrationEntryService = migrationEntryService;
            _databaseContext = umbracoContext;
        }

        public void HandleMigration(string migrationName, Version targetVersion)
        {
            var currentVersion = new SemVersion(0, 0, 0);
            var targetSemVersion = new SemVersion(targetVersion);

            // get all migrations for "Statistics" already executed
            var migrations = _migrationEntryService.GetAll(migrationName);

            // get the latest migration for "Statistics" executed
            var latestMigration = migrations
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            if (latestMigration != null)
                currentVersion = latestMigration.Version;

            if (targetSemVersion == currentVersion)
                return;

            var migrationsRunner = new MigrationRunner(
                _migrationEntryService,
                _logger,
                currentVersion,
                targetSemVersion,
                migrationName);

            try
            {
                migrationsRunner.Execute(_databaseContext.Database);
            }
            catch (Exception e)
            {
                LogHelper.Error<MigrationEvents>("Error running "+ migrationName + " migration", e);
            }
        }
    }

    
}
