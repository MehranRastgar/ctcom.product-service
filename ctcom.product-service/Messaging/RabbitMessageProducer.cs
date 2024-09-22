using MassTransit;

namespace ctcom.ProductService.Messaging
{
    public class RabbitMessageProducer : IMessageProducer
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RabbitMessageProducer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T message) where T : class
        {
            await _publishEndpoint.Publish(message);
        }
    }
}
