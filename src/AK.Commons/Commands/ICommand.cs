using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AK.Commons.Messaging;

namespace AK.Commons.Commands
{
    public class ExecuteUnitRequest
    {
        
    }

    public class ExecuteUnitResponse
    {
        
    }

    public interface ICommand
    {
        IEnumerable<ICommandUnit> Units { get; }
        string CurrentUnit { get; }
        T State<T>();
    }

    public interface ICommandUnit
    {
        string Name { get; }
        ExecuteUnitResponse Execute(ExecuteUnitRequest request);
    }

    public interface ICommandRepository
    {
        ICommand Get(string id);
        void Put(string id, ICommand command);
    }

    public interface ICommandExecutor
    {
        void Execute(string id, ICommandRepository repository);
    }

    public interface ICommandDispatcher
    {
        void Start(IQueue queue, ICommandRepository repository);
        void Stop();
    }
}