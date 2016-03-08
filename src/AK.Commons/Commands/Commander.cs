using System.Linq;
using AK.Commons.Composition;

namespace AK.Commons.Commands
{
    public interface ICommander
    {
        string Issue(string name, object parameters);
        ICommand Inspect(string id);
        void Nudge(string id);
        void StartEngine();
        void StopEngine();
    }

    public class Commander : ICommander
    {
        private readonly ICommandRepository repository;
        private readonly ICommandEngine engine;
        private readonly IComposer composer;

        public Commander(IComposer composer, ICommandRepository repository, ICommandEngine engine)
        {
            this.composer = composer;
            this.repository = repository;
            this.engine = engine;
        }

        public string Issue(string name, object parameters)
        {
            var command = this.composer.Resolve<ICommand>(name);
            command.Parameters = new CommandParameters(parameters);
            var id = this.repository.NextId();
            command.State = new CommandState(id) {UnitName = command.Definition.UnitNames.First(), UnitState = CommandUnitState.Idle};

            this.repository.Put(id, command);
            this.engine.Invoke(id);

            return id;
        }

        public ICommand Inspect(string id)
        {
            return this.repository.Get(id);
        }

        public void Nudge(string id)
        {
            this.engine.Invoke(id);
        }

        public void StartEngine()
        {
            this.engine.Start();
        }

        public void StopEngine()
        {
            this.engine.Stop();
        }
    }
}