using System;
using AK.Commons.Logging;
using AK.Commons.Messaging;

namespace AK.Commons.Commands
{
    public interface ICommandEngine
    {
        void Start();
        void Stop();
        void Invoke(string commandId, int attempt = 1);
    }

    internal class CommandEngine : ICommandEngine
    {
        private readonly IAppLogger logger;
        private readonly ICommandExecutor executor;
        private readonly ICommandRepository repository;
        private readonly IQueue queue;

        public CommandEngine(IAppLogger logger, ICommandExecutor executor, ICommandRepository repository, IQueue queue)
        {
            this.logger = logger;
            this.executor = executor;
            this.repository = repository;
            this.queue = queue;
        }

        public void Start()
        {
            var ids = this.repository.ListEligibleIds();
            foreach (var id in ids) this.Invoke(id);
            this.queue.MessageReceived = message =>
            {
                try
                {
                    if (message.BodyTypeName != typeof (InvokeCommandRequest).FullName) return;
                    var invokeCommandRequest = message.Body<InvokeCommandRequest>();
                    var invoke = this.executor.Execute(invokeCommandRequest);
                    if (invoke) this.Invoke(invokeCommandRequest.Id, invokeCommandRequest.Attempt);
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex);
                }
            };
            this.queue.Start();
        }

        public void Stop()
        {
            this.queue.Stop();
            this.queue.MessageReceived = null;
        }

        public void Invoke(string commandId, int attempt = 1)
        {
            var message = this.queue.CreateMessage(new InvokeCommandRequest(commandId, attempt));
            this.queue.Send(message);
        }
    }
}