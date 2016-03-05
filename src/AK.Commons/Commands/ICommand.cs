using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AK.Commons.Composition;
using AK.Commons.Messaging;

namespace AK.Commons.Commands
{
    public enum CommandUnitState
    {
        Idle,
        Runnning,
        Done,
        Failed
    }

    public class CommandDefinition
    {
        public CommandDefinition(string name, string[] unitNames, Func<IComposer, string> currentUnitNameFactory = null)
        {
            this.Name = name;
            this.UnitNames = unitNames;
            this.CurrentUnitNameFactory = currentUnitNameFactory;
        }

        public CommandDefinition(byte[] serialized)
        {
            
        }

        public string Name { get; }
        public string[] UnitNames { get; }
        public Func<IComposer, string> CurrentUnitNameFactory { get; }

        public byte[] Serialize()
        {
            return null;
        }
    }

    public class CommandState
    {
        public CommandState(string unitName)
        {
            this.UnitName = unitName;
        }

        public bool Success => !this.Errors.Any();
        public string UnitName { get; }
        public string NextUnitName { get; set; }
        public CommandUnitState UnitState { get; set; }
        public bool RecoveringFromFailure { get; set; }
        public ICollection<string> Errors { get; } = new List<string>();
        public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();
    }

    public class CommandParameters
    {
        public static readonly CommandParameters Empty = new CommandParameters(null);

        public CommandParameters(object parameters)
        {
            this.Values = new Dictionary<string, string>();
            if (parameters == null) return;
        }

        public IDictionary<string, string> Values { get; }
    }

    public interface ICommand
    {
        CommandDefinition Definition { get; }
        CommandState State { get; }
        CommandParameters Parameters { get; }
    }

    public interface ICommandUnit
    {
        void Execute(ICommand command);
    }

    public interface ICommandRepository
    {
        ICommand Get(string id);
        void Put(string id, ICommand command);
        string NextId();
    }

    public interface ICommandExecutor
    {
        void Execute(string id);
    }

    public interface ICommandDispatcher
    {
        void Start();
        void Stop();
        void Invoke(string commandId);
    }

    public interface ICommandIssuer
    {
        string Issue(string name, object parameters);
    }

    public interface IProvider
    {
        ICommandIssuer Issuer { get; }
        ICommandDispatcher Dispatcher { get; }
        ICommandRepository Repository { get; }
    }

    public class CommandIssuer : ICommandIssuer
    {
        private readonly ICommandRepository repository;
        private readonly ICommandDispatcher dispatcher;
        private readonly IComposer composer;

        public CommandIssuer(IComposer composer, ICommandRepository repository, ICommandDispatcher dispatcher)
        {
            this.composer = composer;
            this.repository = repository;
            this.dispatcher = dispatcher;
        }

        public string Issue(string name, object parameters)
        {
            var command = this.composer.Resolve<ICommand>(name);
            var id = this.repository.NextId();

            this.repository.Put(id, command);
            this.dispatcher.Invoke(id);

            return id;
        }
    }

    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandExecutor executor;
        private readonly IQueue queue;

        public CommandDispatcher(ICommandExecutor executor, IQueue queue)
        {
            this.executor = executor;
            this.queue = queue;
        }

        public void Start()
        {
            this.queue.MessageReceived = message =>
            {
                if (message.BodyTypeName != typeof (InvokeCommand).FullName) return;
                var invokeCommand = message.Body<InvokeCommand>();
                this.executor.Execute(invokeCommand.Id);
            };
            this.queue.Start();
        }

        public void Stop()
        {
            this.queue.Stop();
        }

        public void Invoke(string commandId)
        {
            this.queue.Send(new InvokeCommandMessage(commandId));
        }
    }

    public class InvokeCommand
    {
        public string Id { get; set; }
    }

    public class InvokeCommandMessage : IMessage
    {
        private readonly string id;

        public InvokeCommandMessage(string id)
        {
            this.id = id;
        }

        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>();

        public T Body<T>() => (T)Convert.ChangeType(new InvokeCommand {Id = this.id}, typeof(T));

        public string BodyTypeName => typeof (InvokeCommand).FullName;
    }

    public class CommandExecutor : ICommandExecutor
    {
        private readonly IComposer composer;
        private readonly ICommandRepository repository;
        private readonly ICommandDispatcher dispatcher;

        public CommandExecutor(IComposer composer, ICommandRepository repository, ICommandDispatcher dispatcher)
        {
            this.composer = composer;
            this.repository = repository;
            this.dispatcher = dispatcher;
        }

        public void Execute(string id)
        {
            var command = this.repository.Get(id);
            if (command.State.UnitState == CommandUnitState.Done) return;
            if (command.State.UnitState == CommandUnitState.Runnning)
            {
                // Delay.
            }
            if (command.State.UnitState == CommandUnitState.Failed) command.State.RecoveringFromFailure = true;
            command.State.UnitState = CommandUnitState.Runnning;
            var nextUnitName = command.Definition.CurrentUnitNameFactory?.Invoke(this.composer) ?? command.State.NextUnitName;
            if(nextUnitName == null) command.State.UnitState = CommandUnitState.Done;

            this.repository.Put(id, command);

            command.D
            throw new NotImplementedException();
        }
    }
}