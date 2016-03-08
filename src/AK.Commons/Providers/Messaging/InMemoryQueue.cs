using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.Messaging;
using AK.Commons.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace AK.Commons.Providers.Messaging
{
    [Export(typeof (IQueue)), PartCreationPolicy(CreationPolicy.NonShared)]
    [ProviderMetadata("InMemory")]
    public class InMemoryQueue : IQueue, IConfigurableProvider
    {
        private const string QueueNameConfigKey = "QueueName";

        private static readonly LockedObject<IDictionary<string, BlockingCollection<InMemoryMessage>>> QueueHash =
            new LockedObject<IDictionary<string, BlockingCollection<InMemoryMessage>>>(
                new Dictionary<string, BlockingCollection<InMemoryMessage>>());

        private readonly IAppConfig config;

        private bool isConfigured;
        private string queueName;
        private BlockingCollection<InMemoryMessage> queue;
        private Task queueTask;
        private CancellationTokenSource queueTaskCancellationTokenSource;

        [ImportingConstructor]
        public InMemoryQueue([Import] IAppConfig config)
        {
            this.config = config;
        }

        public void AssignConfigKeyPrefix(string configKeyPrefix)
        {
            if (this.isConfigured) return;
            this.isConfigured = true;

            var queueNameKey = $"{configKeyPrefix}.{QueueNameConfigKey}";
            this.queueName = this.config.Get<string>(queueNameKey);

            this.queue = QueueHash.ExecuteRead(x => x.LookFor(this.queueName)).ValueOrDefault;
            if (this.queue != null) return;

            this.queue = new BlockingCollection<InMemoryMessage>();
            QueueHash.ExecuteWrite(x => x[this.queueName] = this.queue);
        }

        public IMessage CreateMessage<T>(T body)
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");

            return new InMemoryMessage(body);
        }

        public Action<IMessage> MessageReceived { get; set; }

        public void Start()
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");
            if (this.queueTask != null) return;

            this.queueTaskCancellationTokenSource = new CancellationTokenSource();
            this.queueTask = Task.Factory.StartNew(() =>
            {
                var more = true;
                while (more)
                {
                    more = this.ProcessMessage();
                }
            }, this.queueTaskCancellationTokenSource.Token);
        }

        public void Stop()
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");

            if (this.queueTask == null) return;

            this.queueTaskCancellationTokenSource.Cancel();
            this.queueTask.Wait();
            this.queueTask.Dispose();
            this.queueTaskCancellationTokenSource.Dispose();
            this.queueTask = null;
            this.queueTaskCancellationTokenSource = null;
        }

        public void Send(IMessage message)
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");

            var inMemoryMessage = message as InMemoryMessage;
            if (inMemoryMessage == null) throw new InvalidOperationException("Invalid message type.");

            this.queue.Add(inMemoryMessage);
        }

        private bool ProcessMessage()
        {
            if (this.queueTaskCancellationTokenSource.IsCancellationRequested) return false;

            var message = this.ExtractMessage();
            if (message == null) return false;
            this.HandleMessage(message);

            return true;
        }

        private IMessage ExtractMessage()
        {
            var token = CancellationToken.None;
            IMessage message = null;
            try
            {
                token = this.queueTaskCancellationTokenSource.Token;
                if (token.IsCancellationRequested) return null;
                message = this.queue.Take(token);
                if (this.queueTaskCancellationTokenSource.IsCancellationRequested) return null;
                if (token.IsCancellationRequested) return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            return message;
        }

        private void HandleMessage(IMessage message)
        {
            try
            {
                this.MessageReceived?.Invoke(message);
            }
            catch
            {
            }
        }

        private class InMemoryMessage : IMessage
        {
            private readonly object body;

            public InMemoryMessage(object body)
            {
                this.body = body;
                this.BodyTypeName = body.GetType().FullName;
            }

            public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>();

            public T Body<T>() => (T) this.body;

            public string BodyTypeName { get; }
        }
    }
}