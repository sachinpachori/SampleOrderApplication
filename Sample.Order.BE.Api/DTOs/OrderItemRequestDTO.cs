using System.Security.Cryptography.Xml;

namespace Sample.Order.BE.Api.DTOs
{
    public class OrderItemRequestDTO
    {
        /// <summary>
        /// The product id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// The order item quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The order item tax total
        /// </summary>
        public decimal TaxTotal { get; set; }
        
        /// <summary>
        /// The order item unit price
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// The order item sub total
        /// </summary>
        public decimal LineTotal { get; set; }

    }
}