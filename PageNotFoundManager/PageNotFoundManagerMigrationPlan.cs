using Umbraco.Core.Migrations;

namespace PageNotFoundManager
{
    public class PageNotFoundManagerMigrationPlan : MigrationPlan
    {
        public const string MigrationName = "page-not-found-manager-migration";

        public PageNotFoundManagerMigrationPlan() : base(MigrationName)
        {
            From(string.Empty)
                .To<PageNotFoundInitialMigration>("page-not-found-manager-migration-001");

        }
    }
}
