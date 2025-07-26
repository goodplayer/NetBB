using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NetBB.Domain.Domains.Post;
using NetBB.Domain.Domains.User;
using NetBB.System.EventBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace NetBB.Infrastructure.EventHandlers
{
    public record EventPostData(long PostId)
    {
    };
    public class CreatingPostInManticoreSearchHandler(NatsConnection connection
        , ILogger<CreatingPostInManticoreSearchHandler> logger) : IEventHandler<NewPostEvent>, IEventHandler<EditPostEvent>
    {
        public async Task HandleEvent(NewPostEvent ev, DomainContainer container)
        {
            var data = JsonSerializer.Serialize(new EventPostData(ev.postId));
            var js = new NatsJSContext(connection);
            var ack = await js.PublishAsync(subject: "posts.created", data: data, opts: new NatsJSPubOpts());
            ack.EnsureSuccess();
        }

        public async Task HandleEvent(EditPostEvent ev, DomainContainer container)
        {
            var data = JsonSerializer.Serialize(new EventPostData(ev.postId));
            var js = new NatsJSContext(connection);
            var ack = await js.PublishAsync(subject: "posts.edited", data: data, opts: new NatsJSPubOpts());
            ack.EnsureSuccess();
        }
    }

    public class CreatingPostInManticoreSearchConsumer(NatsConnection connection
        , ILogger<CreatingPostInManticoreSearchConsumer> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var js = new NatsJSContext(connection);
            await js.CreateStreamAsync(new StreamConfig(name: "posts_manticore_search", subjects: new[] { "posts.created", "posts.edited" }));
            var consumer = await js.CreateConsumerAsync(stream: "posts_manticore_search", config: new ConsumerConfig("posts_manticore_search_consumer"));
            await foreach (var msg in consumer.ConsumeAsync<string>().WithCancellation(stoppingToken))
            {
                try
                {
                    var postData = msg.Data;
                    //TODO save data to manticore search
                    logger.LogError($"Processing {msg.Subject} {postData}...");
                    await msg.AckAsync(cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError("CreatingPostInManticoreSearchConsumer error:{}", ex);
                    await msg.NakAsync(cancellationToken: stoppingToken);
                }
            }
        }
    }
}
