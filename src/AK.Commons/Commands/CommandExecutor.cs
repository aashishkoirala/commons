using System;
using System.Threading;
using AK.Commons.Composition;
using AK.Commons.Logging;

namespace AK.Commons.Commands
{
    internal interface ICommandExecutor
    {
        bool Execute(InvokeCommandRequest request);
    }

    internal class CommandExecutor : ICommandExecutor
    {
        private readonly IComposer composer;
        private readonly IAppLogger logger;
        private readonly ICommandRepository repository;

        public CommandExecutor(IComposer composer, IAppLogger logger, ICommandRepository repository)
        {
            this.composer = composer;
            this.logger = logger;
            this.repository = repository;
        }

        public bool Execute(InvokeCommandRequest request)
        {
            var execute = false;
            var command = this.repository.Get(request.Id);
            if (command == null) return false;

            switch (command.State.UnitState)
            {
                case CommandUnitState.AllDone:
                    return false;

                case CommandUnitState.Done:
                    var nextUnitName = this.ExtractNextUnitName(command);
                    if (nextUnitName == null) command.State.UnitState = CommandUnitState.AllDone;
                    else
                    {
                        command.State.NextUnitName = EvaluateNextUnitName(command);
                        command.State.UnitName = nextUnitName;
                        command.State.UnitState = CommandUnitState.Idle;
                    }
                    break;

                case CommandUnitState.Running:
                    request.Attempt++;
                    if (request.Attempt > 10) return false;
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                    break;

                case CommandUnitState.Failed:
                    command.State.RecoveringFromFailure = true;
                    command.State.UnitState = CommandUnitState.Idle;
                    break;

                case CommandUnitState.Idle:
                    command.State.UnitState = CommandUnitState.Running;
                    command.State.NextUnitName = EvaluateNextUnitName(command);
                    execute = true;
                    break;

                default:
                    return false;
            }

            this.repository.Put(request.Id, command);
            if (execute) this.ExecuteUnit(command);
            this.repository.Put(request.Id, command);
            return true;
        }

        private string ExtractNextUnitName(ICommand command)
            => command.Definition.CurrentUnitNameFactory?.Invoke(this.composer) ?? command.State.NextUnitName;

        private static string EvaluateNextUnitName(ICommand command)
        {
            if (command.Definition.CurrentUnitNameFactory != null) return command.State.UnitName;

            var index = Array.IndexOf(command.Definition.UnitNames, command.State.UnitName);
            if (index < 0 || index >= command.Definition.UnitNames.Length - 1) return null;

            return command.Definition.UnitNames[index + 1];
        }

        private void ExecuteUnit(ICommand command)
        {
            try
            {
                var unit = this.composer.Resolve<ICommandUnit>(command.State.UnitName);
                unit.Execute(command);

                command.State.UnitState = CommandUnitState.Done;
            }
            catch (Exception ex)
            {
                this.logger.Error(ex);
                command.State.Errors.Add(ex.ToString());
                command.State.UnitState = CommandUnitState.Failed;
            }
        }
    }
}