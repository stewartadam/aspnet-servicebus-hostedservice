using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace mvp.Services
{
    public class BusListenerService : IHostedService {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private QueueClient queueClient;
        public BusListenerService(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<BusListenerService>();
            this.configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug($"BusListenerService starting; registering message handler.");
            this.queueClient = new QueueClient(configuration.GetValue<string>("ServiceBusConnectionString"), configuration.GetValue<string>("ServiceBusQueueName"));

            var messageHandlerOptions = new MessageHandlerOptions(e => {
                ProcessError(e.Exception);
                return Task.CompletedTask;
            })
            {
                MaxConcurrentCalls = 3,
                AutoComplete = false
            };
            this.queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug($"BusListenerService stopping.");
            await this.queueClient.CloseAsync();
        }

        protected void ProcessError(Exception e) {
            logger.LogError(e, "Error while processing queue item in BusListenerService.");
        }

        protected async Task ProcessMessageAsync(Message message, CancellationToken token)
        {
            var data = Encoding.UTF8.GetString(message.Body);
            MessageInfo item = JsonConvert.DeserializeObject<MessageInfo>(data);

            // ACK the message right away, since we may take a while
            await this.queueClient.CompleteAsync(message.SystemProperties.LockToken);

            logger.LogDebug($"{item.CorrelationId} | BusListenerService received item.");

            // Take a while - renewing lock fails despite message being completed
            // Exception does not stop execution of this message handler
            await Task.Delay(25000);
            logger.LogDebug($"{item.CorrelationId} | BusListenerService processed item.");
        }
    }
}
