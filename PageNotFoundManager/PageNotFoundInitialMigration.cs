using PageNotFoundManager.Models;
using Umbraco.Core.Migrations;

namespace PageNotFoundManager
{
    public class PageNotFoundInitialMigration : MigrationBase
    {
        public const string MigrationName = "page-not-found-manager-migration-initial";

        public PageNotFoundInitialMigration(IMigrationContext context)
            : base(context)
        { }

        public override void Migrate()
        {
            Logger.Debug(typeof(PageNotFoundInitialMigration), MigrationName);

            if (!TableExists(PageNotFound.TableName))
                Create.Table<PageNotFound>().Do();
        }
    }
}
