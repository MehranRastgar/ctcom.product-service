namespace ctcom.ProductService.Events
{
    public class ProductDeletedEvent
    {
        public Guid ProductId { get; }


        public ProductDeletedEvent(Guid productId)
        {
            ProductId = productId;

        }
    }
}