using System;
using System.Linq;
using System.Threading;
using AK.Commons.Composition;
using AK.Commons.Logging;
using AK.Commons.Threading;

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
        private readonly ILocker locker;

        public CommandExecutor(IComposer composer, IAppLogger logger, ICommandRepository repository, ILocker locker)
        {
            this.composer = composer;
            this.logger = logger;
            this.repository = repository;
            this.locker = locker;
        }

        public bool Execute(InvokeCommandRequest request)
        {
            var command = this.repository.Get(request.Id);
            if (command == null) return false;

            bool execute, invoke;
            using (var handle = this.locker.Lock($"AK.Commons.Command.{request.Id}", 5, TimeSpan.FromMinutes(1)))
            {
                if (!handle.Attained)
                {
                    request.Attempt++;
                    if (request.Attempt > 10) return false;
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                }
                bool update;
                this.PreExecute(request, command, out invoke, out execute, out update);
                if (update) this.repository.Put(request.Id, command);
            }

            if (!execute) return invoke;

            this.ExecuteUnit(command);
            using (var handle = this.locker.Lock($"AK.Commons.Command.{request.Id}", 5, TimeSpan.FromMinutes(1)))
            {
                if (!handle.Attained) return invoke;
                this.repository.Put(request.Id, command);
            }

            return invoke;
        }

        private void PreExecute(InvokeCommandRequest request, ICommand command, out bool invoke, out bool execute, out bool update)
        {
            execute = false;
            invoke = false;
            update = false;

            switch (command.State.UnitState)
            {
                case CommandUnitState.AllDone:
                    return;

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
                    if (request.Attempt > 10) return;
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                    break;

                case CommandUnitState.Failed:
                    command.State.FailedAttempts++;
                    if (command.State.FailedAttempts > 5)
                    {
                        command.State.UnitState = CommandUnitState.Damaged;
                        update = true;
                        invoke = false;
                        return;
                    }
                    command.State.RecoveringFromFailure = true;
                    command.State.UnitState = CommandUnitState.Idle;
                    break;

                case CommandUnitState.Idle:
                    command.State.UnitState = CommandUnitState.Running;
                    command.State.NextUnitName = EvaluateNextUnitName(command);
                    execute = true;
                    break;

                case CommandUnitState.Damaged:
                    command.State.FailedAttempts = 0;
                    command.State.RecoveringFromFailure = true;
                    command.State.UnitState = CommandUnitState.Idle;
                    break;

                default:
                    return;
            }

            update = true;
            invoke = true;
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
                command.State.UnitState = command.State.Errors.Any() ? CommandUnitState.Failed : CommandUnitState.Done;
                if (command.State.Errors.Any()) Console.WriteLine(command.State.Errors.First());
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