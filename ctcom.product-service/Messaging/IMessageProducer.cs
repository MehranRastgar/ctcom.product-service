namespace ctcom.ProductService.Messaging
{
    public interface IMessageProducer
    {
        Task PublishAsync<T>(T message) where T : class;
    }
}
