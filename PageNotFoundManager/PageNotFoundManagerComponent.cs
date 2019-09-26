using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;

namespace PageNotFoundManager
{
    public class PageNotFoundManagerComponent : IComponent
    {
        private readonly IScopeProvider scopeProvider;
        private readonly IMigrationBuilder migrationBuilder;
        private readonly IKeyValueService keyValueService;
        private readonly ILogger logger;

        public PageNotFoundManagerComponent(IScopeProvider scopeProvider,
            IMigrationBuilder migrationBuilder,
            IKeyValueService keyValueService,
            ILogger logger)
        {
            this.scopeProvider = scopeProvider ?? throw new System.ArgumentNullException(nameof(scopeProvider));
            this.migrationBuilder = migrationBuilder ?? throw new System.ArgumentNullException(nameof(migrationBuilder));
            this.keyValueService = keyValueService ?? throw new System.ArgumentNullException(nameof(keyValueService));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }
        public void Initialize()
        {
            var upgrader = new Upgrader(new PageNotFoundManagerMigrationPlan());
            upgrader.Execute(scopeProvider, migrationBuilder, keyValueService, logger);
        }

        public void Terminate()
        {
            
        }
    }
}