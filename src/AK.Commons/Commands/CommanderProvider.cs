using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.Logging;
using AK.Commons.Messaging;
using AK.Commons.Threading;
using System.Collections.Generic;

namespace AK.Commons.Commands
{
    public class CommanderProvider : IProviderSource<ICommander>
    {
        private const string ConfigKeyFormat = "ak.commons.commands.{0}";
        private const string ConfigKeyRepositoryFormat = ConfigKeyFormat + ".repository";
        private const string ConfigKeyQueueFormat = ConfigKeyFormat + ".queue";
        private const string ConfigKeyRepositoryProviderFormat = ConfigKeyRepositoryFormat + ".provider";
        private const string ConfigKeyQueueProviderFormat = ConfigKeyQueueFormat + ".provider";

        private readonly IProviderSource<ICommandRepository> repositoryProvider;
        private readonly IProviderSource<IQueue> queueProvider;
        private readonly IComposer composer;
        private readonly IAppLogger logger;

        private readonly LockedObject<IDictionary<string, ICommander>> commanders =
            new LockedObject<IDictionary<string, ICommander>>(new Dictionary<string, ICommander>());

        public CommanderProvider(IComposer composer, IAppConfig config, IAppLogger logger)
        {
            this.composer = composer;
            this.logger = logger;
            this.repositoryProvider = new ProviderSource<ICommandRepository>(
                ConfigKeyRepositoryFormat, ConfigKeyRepositoryProviderFormat, config, composer);
            this.queueProvider = new ProviderSource<IQueue>(ConfigKeyQueueFormat, ConfigKeyQueueProviderFormat, config, composer);
        }

        public ICommander this[string name] => this.GetCommander(name);

        public ICommander Default => this.GetCommander(null);

        private ICommander CreateCommander(string name)
        {
            var repository = name == null ? this.repositoryProvider.Default : this.repositoryProvider[name];
            var queue = name == null ? this.queueProvider.Default : this.queueProvider[name];
            var executor = new CommandExecutor(this.composer, this.logger, repository);
            var engine = new CommandEngine(this.logger, executor, repository, queue);

            return new Commander(this.composer, repository, engine);
        }

        private ICommander GetCommander(string name)
        {
            var perhapsCommander = this.commanders.ExecuteRead(x => x.LookFor(name ?? "Default"));
            if (perhapsCommander.IsThere) return perhapsCommander.Value;

            var commander = this.CreateCommander(name);

            this.commanders.ExecuteWrite(x => x[name ?? "Default"] = commander);

            return commander;
        }
    }
}