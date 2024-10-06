using System;

namespace ctcom.ProductService.Events
{
    public class ProductDeletedEvent
    {
        public Guid Id { get; set; }

        public ProductDeletedEvent(Guid productId)
        {
            Id = productId;
        }
    }
}
