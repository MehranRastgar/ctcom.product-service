using System;

namespace ctcom.ProductService.Events
{
    public class ProductDeletedEvent
    {
        public Guid ProductId { get; set; }

        public ProductDeletedEvent(Guid productId)
        {
            ProductId = productId;
        }
    }
}
