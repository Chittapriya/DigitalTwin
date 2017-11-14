using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.EventHubs;

namespace DigitalTwinApp.TelemetryReceiver
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class TelemetryReceiver : StatelessService
    {
        private const string EhConnectionString = "";
        private const string EhEntityPath = "";
        private const string EhConsumerGroup = "";
        private const string StorageContainerName = "";
        private const string StorageAccountName = "";
        private const string StorageAccountKey = "";

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        static EventProcessorHost eventProcessorHost;

        public TelemetryReceiver(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            var eventHubConnectionStringBuilder = new EventHubsConnectionStringBuilder(EhConnectionString)
            {
                EntityPath = EhEntityPath
            };

            eventProcessorHost = new EventProcessorHost(
                                            EhEntityPath,
                                            EhConsumerGroup,
                                            eventHubConnectionStringBuilder.ToString(),
                                            StorageConnectionString,
                                            StorageContainerName);

            var eventProcessorOptions = new EventProcessorOptions
            {
                MaxBatchSize = 100,
                PrefetchCount = 10,
                ReceiveTimeout = TimeSpan.FromSeconds(120),
                InitialOffsetProvider = (name) => DateTime.Now.AddMilliseconds(20)
            };

            // Registers the Event Processor Host and starts receiving messages
            eventProcessorHost.RegisterEventProcessorAsync<TelemetryEventProcessor>(eventProcessorOptions).Wait();

            return base.RunAsync(cancellationToken);
        }
    }
}
