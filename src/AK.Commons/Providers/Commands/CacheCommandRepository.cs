using AK.Commons.Caching;
using AK.Commons.Commands;
using AK.Commons.Composition;
using AK.Commons.Configuration;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace AK.Commons.Providers.Commands
{
    [Export(typeof (ICommandRepository)), PartCreationPolicy(CreationPolicy.NonShared), ProviderMetadata("Cache")]
    public class CacheCommandRepository : ICommandRepository, IConfigurableProvider
    {
        private const string CacheReferenceNameConfigKey = "CacheReferenceName";

        private readonly IAppConfig config;
        private readonly IComposer composer;

        private ICache cache;
        private bool isConfigured;

        [ImportingConstructor]
        public CacheCommandRepository([Import] IAppConfig config, [Import] IComposer composer)
        {
            this.config = config;
            this.composer = composer;
        }

        public void AssignConfigKeyPrefix(string configKeyPrefix)
        {
            if (this.isConfigured) return;
            this.isConfigured = true;

            var cacheReferenceName = this.config.Get<string>($"{configKeyPrefix}.{CacheReferenceNameConfigKey}");
            this.cache = this.composer.Resolve<IProviderSource<ICache>>()[cacheReferenceName];
        }

        public string[] ListEligibleIds()
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");

            var eligibleIdsEntry = this.cache.Get<string[]>("CacheCommandRepository.EligibleIds");
            return !eligibleIdsEntry.Exists ? new string[0] : eligibleIdsEntry.Value;
        }

        public ICommand Get(string id)
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");

            var entry = this.cache.Get<CommandCacheObject>($"CacheCommandRepository.Command.{id}");
            if (!entry.Exists) return null;

            var commandCacheObject = entry.Value;
            var command = this.composer.Resolve<ICommand>(commandCacheObject.Name);
            command.State = new CommandState(commandCacheObject.State);
            command.Parameters = new CommandParameters();
            command.Parameters.Deserialize(commandCacheObject.Parameters);

            return command;
        }

        public void Put(string id, ICommand command)
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");

            var commandCacheObject = new CommandCacheObject
            {
                Name = command.Definition.Name,
                State = command.State.Serialize(),
                Parameters = command.Parameters.Serialize()
            };

            var isEligible = command.State.UnitState != CommandUnitState.AllDone && command.State.UnitState != CommandUnitState.Damaged;

            var eligibleIds = this.ListEligibleIds();
            var wasEligible = eligibleIds.Contains(id);
            var hasEligibilityChanged = false;

            if (wasEligible && !isEligible)
            {
                eligibleIds = eligibleIds.Except(new[] {id}).ToArray();
                hasEligibilityChanged = true;
            }
            else if (!wasEligible && isEligible)
            {
                eligibleIds = eligibleIds.Concat(new[] {id}).ToArray();
                hasEligibilityChanged = true;
            }

            this.cache.Put($"CacheCommandRepository.Command.{id}", commandCacheObject);
            if (!hasEligibilityChanged) return;

            this.cache.Put("CacheCommandRepository.EligibleIds", eligibleIds);
        }

        public string NextId()
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");

            return Guid.NewGuid().ToString();
        }

        public class CommandCacheObject
        {
            public string Name { get; set; }
            public byte[] Parameters { get; set; }
            public byte[] State { get; set; }
        }
    }
}